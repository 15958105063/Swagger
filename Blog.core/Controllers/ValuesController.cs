using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.core.Model;
using Blog.core.Test;

using Microsoft.AspNetCore.Mvc;

namespace Blog.core.Controllers
{

    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]")]//路由地址规则
    [ApiController]
    public class ValuesController : ControllerBase//2.1之后，控制器基类都修改为ControllerBase，之前是Controller
    {

        //四种RESTful风格的编程
        //RESTful 风格接口实际情况是，我们在前后端在约定接口的时候，可以约定各种风格的接口，但是，RESTful 接口是目前来说比较流行的，并且在运用中比较方便和常见的接口。
        // GET api/values
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        //查
        // GET api/values/5
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        //插入
        // POST api/values
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="value"></param>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        //更新
        // PUT api/values/5
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        //删除
        // DELETE api/values/5
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }


        /// <summary>
        /// 得到love
        /// </summary>
        /// <param name="love">model实体类参数</param>
        [HttpGet("{love}")]
        public ActionResult<string> GetLove(Love love)
        {
            return "value";
        }

        ///测试隐藏接口
        /// <summary>
        /// 新增love
        /// </summary>
        /// <param name="love">model实体类参数</param>
        [HttpPost("{love}")]
        [ApiExplorerSettings(IgnoreApi =true)]
        public void PostLove(Love love)
        {
            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
       [HttpPost("{name},{pass}")]
        public async Task<object> GetJwtStr(string name, string pass)
        {
            string jwtStr = string.Empty;
            bool suc = false;

            // 获取用户的角色名，请暂时忽略其内部是如何获取的，可以直接用 var userRole="Admin"; 来代替更好理解。
            var userRole = "Admin";//await _sysUserInfoServices.GetUserRoleNameStr(name, pass);
            if (userRole != null)
            {
                // 将用户id和角色名，作为单独的自定义变量封装进 token 字符串中。
                TokenModelJwt tokenModel = new TokenModelJwt { Uid = 1, Role = userRole };
                jwtStr = JwtHelper.IssueJwt(tokenModel);//登录，获取到一定规则的 Token 令牌
                suc = true;
            }
            else
            {
                jwtStr = "login fail!!!";
            }

            return Ok(new
            {
                success = suc,
                token = jwtStr//生成的token
            });
        }


        public ActionResult<IEnumerable<string>> Get

    }
}
