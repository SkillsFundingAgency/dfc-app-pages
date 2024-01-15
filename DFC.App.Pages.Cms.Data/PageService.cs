using DFC.App.Pages.Cms.Data.Interface;
using DFC.App.Pages.Cms.Data.Content;
using DFC.App.Pages.Cms.Data.Model;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace DFC.App.Pages.Cms.Data
{
    
    public class PageService : IPageService
    {
        private readonly ICmsRepo repo;
        private readonly IRedisCMSRepo redisCMSRepo;
        private readonly IConfiguration configuration;
        private contentModeOptions _options;    

        public string status;


        /// <summary>
        /// Initializes a new instance of the <see cref="PageService"/> class.
        /// </summary>
        /// <param name="repo">The repo.</param>
        public PageService(IRedisCMSRepo redisCMSRepo, IConfiguration configuration, IOptions<contentModeOptions> options)
        {
            this.redisCMSRepo = redisCMSRepo;
            this.configuration = configuration;
            _options = options.Value;
        }

       

        public async Task<IList<Model.PageUrl>> GetPageUrls()
    {

            status = _options.value;
           
            
            string query = @$"
                query pageurl ($status: Status = {status}) {{
                    page(status: $status) {{
                       displayText
	                    pageLocation {{
                        urlName
                        fullUrl
                        redirectLocations
                        defaultPageForLocation
                    }}
                    breadcrumb: pageLocations {{
                          termContentItems {{
                            ... on PageLocation {{
                              displayText
                              modifiedUtc
                              breadcrumbText
                            }}
                          }}
                        }}
                    triageToolFilters {{
                        contentItems {{
                        ... on TriageToolFilter {{
                            displayText
                        }}
                        }}
                    }} 
                    }}
                }}";

            var response = await redisCMSRepo.GetGraphQLData<PageUrlReponse>(query, "pages/GetPageUrls");

            return response.Page;

        }

    public async Task<IList<Model.Page>> GetPage(string path)
    {
           
            status = _options.value;
         
            string query = @$"
               query page {{
                  page(status: {status}, first: 1 , where: {{pageLocation: {{url: ""{path}""}}}}) {{
                    displayText
                    description
                    pageLocation {{
                      urlName
                      fullUrl
                      redirectLocations
                      defaultPageForLocation
                    }}
                    breadcrumb: pageLocations {{
                          termContentItems {{
                            ... on PageLocation {{
                              displayText
                              modifiedUtc
                              breadcrumbText
                            }}
                          }}
                        }}
                    showBreadcrumb
                    showHeroBanner
                    herobanner {{
                        html
                    }}
                    useInTriageTool
                    triageToolSummary {{
                      html
                    }}
                    triageToolFilters {{
                      contentItems {{
                        ... on TriageToolFilter {{
                          displayText
                        }}
                      }}
                    }}
                    flow {{
                      widgets {{
                        ... on HTML {{
                          metadata {{
                            alignment
                            size
                          }}
                          htmlBody {{
                            html
                          }}
                        }}
                        
                        ... on HTMLShared {{
                          metadata {{
                            alignment
                            size
                          }}
                          sharedContent {{
                            contentItems {{
                              ... on SharedContent {{
                                displayText
                                content {{
                                  html
                                }}
                              }}
                            }}
                          }}
                        }}
                      }}
                    }}
                  }}
                }}
";

            var response = await redisCMSRepo.GetGraphQLData<PageResponse>(query, "pages/GetPage" + path);



            return response.Page;

        }
    
    public async Task<Model.Item> GetBreadCrumbs(string queryName)
    {
            var response = await redisCMSRepo.GetSqlData<BreadcrumbResponse>(queryName, "pages/GetBreadCrumbs");

            return response.Items.FirstOrDefault();
        }
    
    }
}