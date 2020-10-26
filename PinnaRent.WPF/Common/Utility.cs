#region

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using PinnaRent.Core;
using PinnaRent.Core.Models;
using PinnaRent.DAL;
using PinnaRent.Service;
using WebMatrix.WebData;

#endregion

namespace PinnaRent.WPF
{
    public static class Utility
    {
        public static void InitializeWebSecurity()
        {
            var dbContext2 = DbContextUtil.GetDbContextInstance();
            try
            {
                if (!WebSecurity.Initialized)
                    WebSecurity.InitializeDatabaseConnection(Singleton.ConnectionStringName, Singleton.ProviderName,
                        "Users",
                        "UserId", "UserName", autoCreateTables: false);

                /*************************/
                IList<RoleDTO> listOfRoles = CommonUtility.GetRolesList();
                var lofRoles2 = new UserService(true).GetAllRoles().ToList();
                if (listOfRoles.Count != lofRoles2.Count)
                {
                    foreach (var role in listOfRoles)
                    {
                        var roleFound = lofRoles2.Any(role2 => role2.RoleName == role.RoleName);
                        if (!roleFound)
                            dbContext2.Set<RoleDTO>().Add(role);
                    }
                    dbContext2.SaveChanges();
                }
                /*************************/

                if (!new UserService(true).GetAll().Any())
                {
                    #region Seed Default Roles and Users

                    WebSecurity.CreateUserAndAccount("superadmin", "P@ssw0rd1!",
                        new
                        {
                            Status = 1,
                            Enabled = true,
                            RowGuid = Guid.NewGuid(),
                            Email = "superadmin@amihanit.com",
                            CreatedByUserId = 1,
                            DateRecordCreated = DateTime.Now,
                            ModifiedByUserId = 1,
                            DateLastModified = DateTime.Now
                        });

                    WebSecurity.CreateUserAndAccount("adminuser", "P@ssw0rd",
                        new
                        {
                            Status = 0,
                            Enabled = true,
                            RowGuid = Guid.NewGuid(),
                            Email = "adminuser@amihanit.com",
                            CreatedByUserId = 1,
                            DateRecordCreated = DateTime.Now,
                            ModifiedByUserId = 1,
                            DateLastModified = DateTime.Now
                        });

                    WebSecurity.CreateUserAndAccount("PinnaRent01", "PinnaRent02",
                        new
                        {
                            Status = 0,
                            Enabled = true,
                            RowGuid = Guid.NewGuid(),
                            Email = "PinnaRent@amihanit.com",
                            CreatedByUserId = 1,
                            DateRecordCreated = DateTime.Now,
                            ModifiedByUserId = 1,
                            DateLastModified = DateTime.Now
                        });

                    //add row guid for membership table members
                    var members = new UserService().GetAllMemberShips();
                    foreach (var membershipDTO in members)
                    {
                        membershipDTO.RowGuid = Guid.NewGuid();
                        membershipDTO.Enabled = true;
                        membershipDTO.CreatedByUserId = 1;
                        membershipDTO.DateRecordCreated = DateTime.Now;
                        membershipDTO.ModifiedByUserId = 1;
                        membershipDTO.DateLastModified = DateTime.Now;
                        dbContext2.Set<MembershipDTO>().Add(membershipDTO);
                        dbContext2.Entry(membershipDTO).State = EntityState.Modified;
                    }
                    dbContext2.SaveChanges();

                    var lofRoles = new UserService().GetAllRoles().ToList();
                    foreach (var role in lofRoles)
                    {
                        dbContext2.Set<UsersInRoles>().Add(new UsersInRoles
                        {
                            RoleId = role.RoleId,
                            UserId = WebSecurity.GetUserId("superadmin")
                        });
                    }

                    foreach (var role in lofRoles.Skip(0))
                    {
                        dbContext2.Set<UsersInRoles>().Add(new UsersInRoles
                        {
                            RoleId = role.RoleId,
                            UserId = WebSecurity.GetUserId("adminuser")
                        });
                    }

                    foreach (var role in lofRoles.Skip(0))
                    {
                        dbContext2.Set<UsersInRoles>().Add(new UsersInRoles
                        {
                            RoleId = role.RoleId,
                            UserId = WebSecurity.GetUserId("PinnaRent01")
                        });
                    }

                    dbContext2.SaveChanges();

                    #endregion
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Problem on InitializeWebSecurity" +
                                Environment.NewLine + ex.Message +
                                Environment.NewLine + ex.InnerException);
            }
            finally
            {
                dbContext2.Dispose();
            }
        }
    }

    //public interface ILogger
    //{
    //    void Information(string message);
    //    void Information(string fmt, params object[] vars);
    //    void Information(Exception exception, string fmt, params object[] vars);
    //    void Warning(string message);
    //    void Warning(string fmt, params object[] vars);
    //    void Warning(Exception exception, string fmt, params object[] vars);
    //    void Error(string message);
    //    void Error(string fmt, params object[] vars);
    //    void Error(Exception exception, string fmt, params object[] vars);
    //    void TraceApi(string componentName, string method, TimeSpan timespan);
    //    void TraceApi(string componentName, string method, TimeSpan timespan, string properties);
    //    void TraceApi(string componentName, string method, TimeSpan timespan, string fmt, params object[] vars);
    //}

    //public class Logger : ILogger
    //{
    //    public void Information(string message)
    //    {
    //        Trace.TraceInformation(message);
    //    }

    //    public void Information(string fmt, params object[] vars)
    //    {
    //        Trace.TraceInformation(fmt, vars);
    //    }

    //    public void Information(Exception exception, string fmt, params object[] vars)
    //    {
    //        Trace.TraceInformation(FormatExceptionMessage(exception, fmt, vars));
    //    }

    //    public void Warning(string message)
    //    {
    //        Trace.TraceWarning(message);
    //    }

    //    public void Warning(string fmt, params object[] vars)
    //    {
    //        Trace.TraceWarning(fmt, vars);
    //    }

    //    public void Warning(Exception exception, string fmt, params object[] vars)
    //    {
    //        Trace.TraceWarning(FormatExceptionMessage(exception, fmt, vars));
    //    }

    //    public void Error(string message)
    //    {
    //        Trace.TraceError(message);
    //    }

    //    public void Error(string fmt, params object[] vars)
    //    {
    //        Trace.TraceError(fmt, vars);
    //    }

    //    public void Error(Exception exception, string fmt, params object[] vars)
    //    {
    //        Trace.TraceError(FormatExceptionMessage(exception, fmt, vars));
    //    }

    //    public void TraceApi(string componentName, string method, TimeSpan timespan)
    //    {
    //        TraceApi(componentName, method, timespan, "");
    //    }

    //    public void TraceApi(string componentName, string method, TimeSpan timespan, string fmt, params object[] vars)
    //    {
    //        TraceApi(componentName, method, timespan, string.Format(fmt, vars));
    //    }

    //    public void TraceApi(string componentName, string method, TimeSpan timespan, string properties)
    //    {
    //        string message = String.Concat("Component:", componentName, ";Method:", method, ";Timespan:",
    //            timespan.ToString(), ";Properties:", properties);
    //        Trace.TraceInformation(message);
    //    }

    //    private static string FormatExceptionMessage(Exception exception, string fmt, object[] vars)
    //    {
    //    // Simple exception formatting: for a more comprehensive version see
    //    // http://code.msdn.microsoft.com/windowsazure/Fix-It-app-for-Building-cdd80df4
    //                var sb = new StringBuilder();
    //                sb.Append(string.Format(fmt, vars));
    //                sb.Append(" Exception: ");
    //                sb.Append(exception.ToString());
    //                return sb.ToString();
    //    }
    //}
}