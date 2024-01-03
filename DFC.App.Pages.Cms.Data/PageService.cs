using DFC.App.Pages.Cms.Data.Interface;
using DFC.App.Pages.Cms.Data.Model;

namespace DFC.App.Pages.Cms.Data
{
    public class PageService : IPageService
    {
        private readonly ICmsRepo repo;

        /// <summary>
        /// Initializes a new instance of the <see cref="PageService"/> class.
        /// </summary>
        /// <param name="repo">The repo.</param>
        public PageService(ICmsRepo repo)
    {
        this.repo = repo;
    }

    public async Task<IList<Model.PageUrl>> GetPageUrls()
    {
            string query = @$"
                query pageurl {{
                    page(status: PUBLISHED) {{
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

            var response = await repo.GetData<PageUrlReponse>(query);

            return response.Page;

        }

    public async Task<IList<Model.Page>> GetPage(string path)
    {
            string query = @$"
               query page {{
                  page(status: PUBLISHED, first: 1 , where: {{pageLocation: {{url: ""{path}""}}}}) {{
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

            var response = await repo.GetData<PageResponse>(query);

            return response.Page;

        }
    
    public async Task<Model.Item> GetBreadCrumbs(string queryName)
    {
            var response = await repo.GetSqlData<BreadcrumbResponse>(queryName);

            return response.Items.FirstOrDefault();
        }
    
    }
}