using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Blog.core.Test;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;
using Swashbuckle.AspNetCore.Swagger;
//using Microsoft.Extensions.PlatformAbstractions;//需要引用Nuget包

namespace Blog.core
{
    /// <summary>
    /// 
    /// </summary>
    public class Startup
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// 
        /// </summary>
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        //ConfigureServices方法是用来把services(各种服务, 例如identity, ef, mvc等等包括第三方的, 或者自己写的)加入(register)到container(asp.net core的容器)中去,
        //并配置这些services. 这个container是用来进行dependency injection的(依赖注入). 所有注入的services(此外还包括一些框架已经注册好的services) 在以后写代码的时候,
        //都可以将它们注入(inject)进去. 例如上面的Configure方法的参数, app, env, loggerFactory都是注入进去的services.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2); // 注册MVC到Container

            #region Swagger

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v0.1.0",
                    Title = "Blog.core API",
                    Description = "框架说明文档",
                    TermsOfService = "None",
                    Contact = new Contact
                    {
                        Name = "Blog.core",
                        Email = "744310077@qq.com",
                        Url = "https://www.jianshu.com/u/94102b59cc2a"
                    }
                });


                //在AddSwaggerGen内部
                #region 读取xml信息

                //using Microsoft.Extensions.PlatformAbstractions 需要进行引用
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var xmlPath = Path.Combine(basePath, "Blog.core.xml");//这个就是刚刚配置的xml文件名
                c.IncludeXmlComments(xmlPath, true);//默认的第二个参数是false，这个是controller的注释，记得修改

                var xmlModelPath = Path.Combine(basePath, "Blog.core.Model.xml");//这个就是Model层的xml文件名
                c.IncludeXmlComments(xmlModelPath, true);
                #endregion


                #region token绑定到ConfigureServices
                //添加header验证信息
                //c.OperationFilter<swaggerHeader>();
                var security = new Dictionary<string, IEnumerable<string>>
                {
                    {"Blog.core", new string[] { }},

                };
                c.AddSecurityRequirement(security);

                //方案名称"Blog.core"可自定义，上下一致即可
                c.AddSecurityDefinition("Blog.core", new ApiKeyScheme
                {
                    Description = "JWT授权(数据将在请求头中进行传输) 直接在下框中输入Bearer {token}（注意两者之间是一个空格）",
                    Name = "Authorization", //jwt默认的参数名称
                    In = "header", //jwt默认存放Authorization信息的位置(请求头中)
                    Type = "apiKey"

                });

                #endregion


            });

            #endregion


            #region Authorize

            //基于策略的授权机制：
            //[HttpGet]
            //    [Authorize(Roles = "Admin")]
            //    [Authorize(Roles = "System")]
            //public ActionResult<IEnumerable<string>> GetLogin()
            //{

            //    return new string[] { "value1", "value2" };
            //}
        // 1【授权】、这个和上边的异曲同工，好处就是不用在controller中，写多个 roles 。
        // 然后这么写 [Authorize(Policy = "Admin")]
        services.AddAuthorization(option =>
            {
                option.AddPolicy("Client",policy=>policy.RequireRole("Client").Build());
                option.AddPolicy("Admin", policy => policy.RequireRole("Admin").Build());
                option.AddPolicy("SystemOrAdmin", policy => policy.RequireRole("Admin","System"));

            });

            #endregion
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        //Configure方法是asp.net core程序用来具体指定如何处理每个http请求的, 例如我们可以让这个程序知道我使用mvc来处理http请求,
        //那就调用app.UseMvc()这个方法就行. 但是目前, 所有的http请求都会导致返回"Hello World!".
        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                //开发环境中，使用异常页面，这样可以暴漏错误堆栈信息，所以不要放在生产环境
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");

            }

            #region Swagger

 

            app.UseSwagger();//需要引用nuget程序包

            app.UseSwaggerUI(c =>
                {
                    //之前是写死的
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ApiHelp V1");
                    c.RoutePrefix = "";//路径配置，设置为空，表示直接在根域名（localhost:8001）访问该文件,注意localhost:8001/swagger是访问不到的，去launchSettings.json把launchUrl去掉

                    //根据版本名称倒序 遍历展示
                    //typeof(Version).GetEnumNames().OrderByDescending(e => e).ToList().ForEach(version =>
                    //    {
                    //        c.SwaggerEndpoint($"/swagger/{version}/swagger.json", $"{""} {version}");
                    //    });

                }
            );//需要引用nuget程序包
            #endregion

            #region Authen
            //app.UseMiddleware<JwtTokenAuth>();//注意此授权方法已经放弃，请使用下边的官方验证方法。但是如果你还想传User的全局变量，还是可以继续使用中间件

            //app.UseAuthentication();

            //自定义认证中间件
            app.UseJwtTokenAuth(); //也可以app.UseMiddleware<JwtTokenAuth>();
            #endregion

            #region CORS

            //跨域第一种版本，请要ConfigureService中配置服务 services.AddCors();
            //    app.UseCors(options => options.WithOrigins("http://localhost:8021").AllowAnyHeader()
            //.AllowAnyMethod()); 

            //跨域第二种方法，使用策略，详细策略信息在ConfigureService中
            app.UseCors("LimitRequests");//将 CORS 中间件添加到 web 应用程序管线中, 以允许跨域请求
            #endregion


            #region 跳转Https
            app.UseHttpsRedirection();
            #endregion

            #region 使用静态文件
            app.UseStaticFiles();
            #endregion


            #region 使用cookie
            app.UseCookiePolicy();
            #endregion

            #region 返回错误码
            app.UseStatusCodePages();//把错误码返回前台，比如是404
            #endregion


            app.UseMvc();
        }
    }
}
