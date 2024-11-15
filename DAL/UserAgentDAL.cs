using DAL.Generic;
using Entities.Models;
using Entities.ViewModels.UserAgent;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;
using Utilities.Contants;

namespace DAL
{
   public class UserAgentDAL : GenericService<UserAgent>
    {
        public UserAgentDAL(string connection) : base(connection) { }
        public int CreateUserAgent(UserAgent model)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {

                    var deta = _DbContext.UserAgent.Add(model);
                    _DbContext.SaveChanges();


                }
                return 1;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreateUserAgent - UserAgentDAL: " + ex);
                return 0;
            }
        }
        public int UpdataUserAgent(int ClientId,int UserId,int create_id)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var model = _DbContext.UserAgent.FirstOrDefault(s => s.ClientId == ClientId);
                    var modelclient = _DbContext.Client.FirstOrDefault(s => s.Id == ClientId);
                    if (model != null) { 
                    model.UserId = UserId;
                    model.UpdateLast = DateTime.Now;
                    modelclient.UpdateTime = DateTime.Now;
                    var deta = _DbContext.UserAgent.Update(model);
                    var Updateclient = _DbContext.Client.Update(modelclient);
                    _DbContext.SaveChanges();
                    }
                    else
                    {
                        var model2=new UserAgent();
                        model2.CreateDate = DateTime.Now;
                        model2.UpdateLast = DateTime.Now;
                        model2.VerifyDate = DateTime.Now;
                        model2.VerifyStatus = VerifyStatus.DA_DUYET;
                        model2.ClientId = ClientId;
                        model2.UserId = UserId;
                        model2.CreatedBy = create_id;
                        var deta = _DbContext.UserAgent.Add(model2);
                        _DbContext.SaveChanges();
                    }

                }
                return 1;
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdataUserAgent - UserAgentDAL: " + ex);
                return 0;
            }
        }

        public UserAgentViewModel UserAgentByClient(int ClientId)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var model =(from a in _DbContext.UserAgent.Where(s => s.ClientId == ClientId)
                                join b in _DbContext.User on a.UserId equals b.Id
                                select new UserAgentViewModel
                                {
                                    Id=a.Id,
                                    UserId=a.UserId,
                                    ClientId=a.ClientId,
                                    UserId_Name=b.FullName,
                                }).FirstOrDefault();


                    return model;
                }
                
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UserAgentByClient - UserAgentDAL: " + ex);
                return null;
            }
        }
        public UserAgent GetUserAgentClient(int ClientId)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var model = _DbContext.UserAgent.FirstOrDefault(s => s.ClientId == ClientId);
                    return model;
                }
               
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetUserAgentClient - UserAgentDAL: " + ex);
                return null;
            }
        }
        public int UpdataUserAgentClient(UserAgent model)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var deta = _DbContext.UserAgent.Update(model);
                    _DbContext.SaveChanges();
                    return 1;
                }

            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdataUserAgentClient - UserAgentDAL: " + ex);
                return 0;
            }
        }
    }
}
