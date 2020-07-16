param
(
    [string]$baseDir="F:\dredd\servicetaxonomy",
    [string]$SubscriptionKey="############################",
    [string]$SourceFile="C:\Users\esfa\Downloads\Service Taxonomy Get Skills By Label.openapi_origin.yaml",
    [string]$httpProtocol="https",
    #[string]$ApiBaseUrl="localhost:7071",
    [string[]] $httpStatusCodeExclusions = ( '204','400','422','500')
)

Function iIf($If, $Right, $Wrong) {If ($If) {$Right} Else {$Wrong}}
Function getIndent ( $Text ) { return iIF([string]::IsNullOrEmpty($Text)){ 0, $Text.Length - $Text.TrimStart().Length } }

#clear
Write-Output "baseDir:" + $baseDir;
Write-Output "ApiBaseUri:" + $ApiBaseUri;

cd $baseDir

if ( Test-Path -path convertedfile.yaml )
{
    Remove-Item convertedfile.yaml
}

$converted = $( api-spec-converter -f openapi_3 -t swagger_2 -s yaml $SourceFile)

$outputLine = $true
$suppressText = $false

# deal with status codes we don't want to process
ForEach ($line in $($converted -split "`r`n"))
{
    $startOfSkipSection = $false
    $indent = getIndent -Text $Line
    

    if ( $suppressText -And -Not $startOfSkipSection -And $Line.trim().EndsWith(":") -And  $startIndent -ge $indent )
    {
        $suppressText = $false
    }
    if ( -not $suppressText )
    {
        foreach ( $item in $httpStatusCodeExclusions )
        {
            If ( $Line.contains($item) -And $Line.trim().EndsWith(":") ) 
            {
                $startIndent = getIndent -Text $Line
                # current line is start of session we want to skip
                $suppressText = $true
                $startOfSkipSection = $true
            }
        } 
    }
    
    if ( -not($suppressText ) )
    {
        $Line | Out-File -Append -Encoding ascii convertedfile.yaml
        #Write-host $Line
    }
}

$baseUrlLine = $converted -match '^host:\s[a-zA-Z.]+'
$baseUrl = ($baseUrlLine -split ":")[1]

if ($baseUrl.length -lt 1)
{
    $baseUrl="http://localhost:7071"
}

if ( ! $baseUrl.Contains("http") )
{
   #add http protocol
   $baseUrl = $httpProtocol + "://" + $baseUrl.Trim()
}

$header="Ocp-Apim-Subscription-Key:" + $SubscriptionKey

Write-Host "Run dredd against converted file: "$baseUrl 
dredd convertedfile.yaml --header $header $baseUrl

Remove-Item convertedfile.yaml