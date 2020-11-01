using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

namespace Service.Controllers
{
    [Route("api/[controller]")]
    public class WorkController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public WorkController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        public ActionResult Get()
        {
            return Json(DAL.WorkInfo.Instance.GetCount());
        }
        [HttpGet("new")]
        public ActionResult GetNew()
        {
            var result = DAL.WorkInfo.Instance.GetNew();
            if (result.Count() != 0)
                return Json(Result.Ok(result));
            else
                return Json(Result.Err("记录数为0"));
        }
        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            var result = DAL.Activity.Instance.GetModel(id);
            if (result != null)
                return Json(Result.Ok(result));
            else
                return Json(Result.Err("WorkId不存在"));
        }
        [HttpPost]
        public ActionResult Post([FromBody]Model.WorkInfo workinfo)
        {
            workinfo.recommend = "否";
            workinfo.workVerify = "待审核";
            workinfo.uploadTime = DataTime.Now;
            try{
                int n = DAL.WorkInfo.Instance.Add(workInfo);
                return Json(Result.Ok("发布作品成功",n));
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("foreign key"))
                    if (ex.Message.ToLower().Contains("username"))
                        return Json(Result.Err("合法用户才能添加记录"));
                else
                        return Json(Result.Err("作品所属活动不存在"));
                else if (ex.Message.ToLower().Contains("null"))
                    return Json(Result.Err("作品名称，作品图片，上传作品时间，作品审核情况，用户名，是否推荐不能为空"));
                else
                    return Json(Result.Err(ex.Message));
            }
        }
        [HttpPut]
        public ActionResult Put([FromBody]Model.WorkInfo workinfo)
        {
            workinfo.recommend = "否";
            workinfo.workVerify = "待审核";
            workinfo.uploadTime = DataTime.Now;
            try
            {
                int n = DAL.WorkInfo.Instance.Add(workInfo);
                if (n != 0)
                    return Json(Result.Ok("修改作品成功", workInfo.workId));
                else
                    return Json(Result.Err("workId不存在"));
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("null"))
                    return Json(Result.Err("作品名称，作品图片，上传作品时间，作品审核情况，用户名，是否推荐不能为空"));
                else
                    return Json(Result.Err(ex.Message));
            }
        }
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            try
            {
                var n = DAL.WorkInfo.Instance.Add(id);
                if (n != 0)
                    return Json(Result.Ok("删除成功"));
                else
                    return Json(Result.Err("workId不存在"));
            }
            catch (Exception ex)
            {
                return Json(Result.Err(ex.Message));
            }
        }
        [HttpPost("count")]
        public ActionResult getCount([FromBody]int[] acttivityIds)
        {
            return Json(DAL.WorkInfo.Instance.GetCount(acttivityIds));
        }
        [HttpPost("page")]
        public ActionResult getPage([FromBody]Model.WorkInfo page)
        {
            var result = DAL.WorkInfo.Intance.GetPage(page);
            if(result.Count()==0)
                return Json(Result.Err("返回记录书为0"));
            else
                return Json(Result.Ok("result"));
        }
        [HttpGet("findCount")]
        public ActionResult getfindCount(string userName)
        {
            if (findName == null) findName = "";
            return Json(DAL.WorkInfo.Intance.GetFindCount(findName));
        }
        [HttpGet("myCount")]
        public ActionResult getMyCount(string userName)
        {
            if (userName == null) userName = "";
            return Json(DAL.WorkInfo.Intance.GetMyCount(userName));
        }
        [HttpGet("findPage")]
        public ActionResult getFindPage([FromBody]Model.WorkFindPage page)
        {
            if (getPage.workName == null) page.workName = "";
            var result = DAL.WorkInfo.Intance.GetFindPage(page);
            if (result.Count() == 0)
                return Json(Result.Err("返回记录书为0"));
            else
                return Json(Result.Ok("result"));
        }
        [HttpGet("myPage")]
        public ActionResult getMyPage([FromBody]Model.WorkFindPage page)
        {
            if (getPage.userName == null) page.userName = "";
            var result = DAL.WorkInfo.Intance.GetMyPage(page);
            if (result.Count() == 0)
                return Json(Result.Err("返回记录书为0"));
            else
                return Json(Result.Ok("result"));
        }
        [HttpPut("Verify")]
        public ActionResult PutVerify([FromBody]Model.WorkInfo workInfo)
        {
            try
            {
                var n = DAL.WorkInfo.Instance.UpdateVerify(workInfo);
                if (n != 0)
                    return Json(Result.Ok("审核作品成功",workInfo.workId));
                else
                    return Json(Result.Err("workId不存在"));
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("null"))
                    return Json(Result.Err("作品审核情况不能为空"));
                else
                    return Json(Result.Err(ex.Message));
            }
        }
        [HttpPut("Recommend")]
        public ActionResult PutRecommend([FromBody]Model.WorkInfo workInfo)
        {
            try
            {
                var re = "";
                if (workinfo.recommend == "否") re = "取消";
                var n = DAL.WorkInfo.Instance.UpdateRecommend(workInfo);
                if (n != 0)
                    return Json(Result.Ok($"{re}推荐作品成功", workInfo.workId));
                else
                    return Json(Result.Err("workId不存在"));
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("null"))
                    return Json(Result.Err("推荐作品情况不能为空"));
                else
                    return Json(Result.Err(ex.Message));
            }
        }
        [HttpPut("{id}")]
        public ActionResult upImg(string id,List<IFromFile> files)
        {
            var path = System.IO.Path.Combine(_hostingEnvironment.WebRootPath, "img", "Work");
            var fileName = $"{path}/{id}";
            try
            {
                var ext = DAL.Upload.Instance.UpImg(files[0], fileName);
                if (ext == null)
                    return Json(Result.Err("请上传图片文件"));
                else
                {
                    var file = $"img/Work.{id}{ext}";
                    Model.WorkInfo workInfo = new Model.WorkInfo();
                    if (id.StartsWith("i"))
                    {
                        workInfo.worId = int.Parse(id.Substring(1));
                        workInfo.workIntroduction = file;
                    }
                    else
                    {
                        workInfo.workId = int.Parse(id.Substring(1));
                        workInfo.workPicture = file;
                    }
                    var n = DAL.WorkInfo.Instance.UpdateImg(workInfo);
                    if(n>0)
                        return Json(Result.Ok("上传成功",file));
                    else
                        return Json(Result.Err("请输入正确作品id"));
                }
            }
            catch(Exception ex)
            {
                return Json(Result.Err(ex.Message));
            }
        }
    }
}
