﻿#region Copyright
//
// DotNetNuke® - http://www.dnnsoftware.com
// Copyright (c) 2002-2017
// by DotNetNuke Corporation
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions
// of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dnn.ExportImport.Components.Common;
using Dnn.ExportImport.Components.Entities;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Security.Permissions;
using PlatformDataProvider = DotNetNuke.Data.DataProvider;

namespace Dnn.ExportImport.Components.Providers
{
    public class DataProvider
    {
        #region Shared/Static Methods

        private static readonly DataProvider Provider;

        static DataProvider()
        {
            Provider = new DataProvider();
        }

        public static DataProvider Instance()
        {
            return Provider;
        }

        private DataProvider()
        {
            // so it can't be instantiated outside this class
        }

        #endregion

        #region Public Methods

        public int AddNewJob(int portalId, int userId, JobType jobType, string exportFile, string serializedObject)
        {
            return PlatformDataProvider.Instance().ExecuteScalar<int>(
                "ExportImportJobs_Add", portalId, (int)jobType, userId, exportFile, serializedObject);
        }

        public void UpdateJobStatus(int jobId, JobStatus jobStatus)
        {
            DateTime? completeDate = null;
            if (jobStatus == JobStatus.DoneFailure || jobStatus == JobStatus.DoneSuccess)
                completeDate = DateTime.UtcNow;

            PlatformDataProvider.Instance()
                .ExecuteNonQuery("ExportImportJobs_UpdateStatus", jobId, jobStatus, completeDate);
        }

        public void SetJobCancelled(int jobId)
        {
            PlatformDataProvider.Instance().ExecuteNonQuery("ExportImportJobs_SetCancelled", jobId);
        }

        public void RemoveJob(int jobId)
        {
            // using 60 sec timeout because cascading deletes in logs might take a lot of time
            PlatformDataProvider.Instance().ExecuteNonQuery(60, "ExportImportJobs_Remove", jobId);
        }

        public IDataReader GetFirstActiveJob()
        {
            return PlatformDataProvider.Instance().ExecuteReader("ExportImportJobs_FirstActive");
        }

        public IDataReader GetJobById(int jobId)
        {
            return PlatformDataProvider.Instance().ExecuteReader("ExportImportJobs_GetById", jobId);
        }

        public IDataReader GetJobSummaryLog(int jobId)
        {
            return PlatformDataProvider.Instance().ExecuteReader("ExportImportJobLogs_Summary", jobId);
        }

        public IDataReader GetJobFullLog(int jobId)
        {
            return PlatformDataProvider.Instance().ExecuteReader("ExportImportJobLogs_Full", jobId);
        }

        public IDataReader GetAllJobs(int? portalId, int? pageSize, int? pageIndex)
        {
            return PlatformDataProvider.Instance()
                .ExecuteReader("ExportImportJobs_GetAll", portalId, pageSize, pageIndex);
        }

        public IDataReader GetJobChekpoints(int jobId)
        {
            return PlatformDataProvider.Instance().ExecuteReader("ExportImportCheckpoints_GetByJob", jobId);
        }

        public void UpsertJobChekpoint(ExportImportChekpoint checkpoint)
        {
            PlatformDataProvider.Instance().ExecuteNonQuery("ExportImportCheckpoints_Upsert",
                checkpoint.JobId, checkpoint.Category, checkpoint.Stage, checkpoint.StageData);
        }

        public IDataReader GetAllScopeTypes()
        {
            return PlatformDataProvider.Instance().ExecuteReader("ExportTaxonomy_ScopeTypes");
        }

        public IDataReader GetAllVocabularyTypes()
        {
            return PlatformDataProvider.Instance().ExecuteReader("ExportTaxonomy_VocabularyTypes");
        }

        public IDataReader GetAllTerms(DateTime? sinceDate)
        {
            return PlatformDataProvider.Instance().ExecuteReader("ExportTaxonomy_Terms", sinceDate);
        }

        public IDataReader GetAllVocabularies(DateTime? sinceDate)
        {
            return PlatformDataProvider.Instance().ExecuteReader("ExportTaxonomy_Vocabularies", sinceDate);
        }

        public IDataReader GetAllRoleGroups(int portalId, DateTime? sinceDate)
        {
            return PlatformDataProvider.Instance().ExecuteReader("Export_RoleGroups", portalId, sinceDate);
        }

        public IDataReader GetAllRoles(int portalId, DateTime? sinceDate)
        {
            return PlatformDataProvider.Instance().ExecuteReader("Export_Roles", portalId, sinceDate);
        }

        public IDataReader GetAllRoleSettings(int portalId, DateTime? sinceDate)
        {
            return PlatformDataProvider.Instance().ExecuteReader("Export_RoleSettings", portalId, sinceDate);
        }

        public IDataReader GetPropertyDefinitionsByPortal(int portalId, bool includeDeleted, DateTime? sinceDate)
        {
            return PlatformDataProvider.Instance()
                .ExecuteReader("Export_GetPropertyDefinitionsByPortal", portalId, includeDeleted, sinceDate);
        }

        public void UpdateRoleGroupChangers(int roleGroupId, int createdBy, int modifiedBy)
        {
            PlatformDataProvider.Instance().ExecuteNonQuery(
                "Export_UpdateRoleGroupChangers", roleGroupId, createdBy, modifiedBy);
        }

        public void UpdateRoleChangers(int roleId, int createdBy, int modifiedBy)
        {
            PlatformDataProvider.Instance().ExecuteNonQuery(
                "Export_UpdateRoleChangers", roleId, createdBy, modifiedBy);
        }

        public void UpdateRoleSettingChangers(int roleId, string settingName, int createdBy, int modifiedBy)
        {
            PlatformDataProvider.Instance().ExecuteNonQuery(
                "Export_UpdateRoleSettingChangers", roleId, settingName, createdBy, modifiedBy);
        }

        public IDataReader GetAllUsers(int portalId, int pageIndex, int pageSize, bool includeDeleted,
            DateTime? sinceDate)
        {
            return PlatformDataProvider.Instance()
                .ExecuteReader("Export_GetAllUsers", portalId, pageIndex, pageSize, includeDeleted, sinceDate);
        }

        public IDataReader GetAspNetUser(string username)
        {
            return PlatformDataProvider.Instance().ExecuteReader("Export_GetAspNetUser", username);
        }

        public IDataReader GetUserMembership(Guid userId, Guid applicationId)
        {
            return PlatformDataProvider.Instance().ExecuteReader("Export_GetUserMembership", userId, applicationId);
        }

        public IDataReader GetUserRoles(int portalId, int userId)
        {
            return PlatformDataProvider.Instance().ExecuteReader("Export_GetUserRoles", portalId, userId);
        }

        public IDataReader GetUserPortal(int portalId, int userId)
        {
            return PlatformDataProvider.Instance().ExecuteReader("Export_GetUserPortal", portalId, userId);
        }

        public IDataReader GetUserAuthentication(int userId)
        {
            return PlatformDataProvider.Instance().ExecuteReader("GetUserAuthentication", userId);
        }

        public IDataReader GetUserProfile(int portalId, int userId)
        {
            return PlatformDataProvider.Instance().ExecuteReader("Export_GetUserProfile", portalId, userId);
        }

        public void UpdateUserChangers(int userId, string createdByUserName, string modifiedByUserName)
        {
            PlatformDataProvider.Instance().ExecuteNonQuery(
                "Export_UpdateUsersChangers", userId, createdByUserName, modifiedByUserName);
        }

        public IDataReader GetPortalSettings(int portalId, DateTime? sinceDate)
        {
            return PlatformDataProvider.Instance().ExecuteReader("Export_GetPortalSettings", portalId, sinceDate);
        }

        public IDataReader GetPortalLanguages(int portalId, DateTime? sinceDate)
        {
            return PlatformDataProvider.Instance().ExecuteReader("Export_GetPortalLanguages", portalId, sinceDate);
        }

        public IDataReader GetPortalLocalizations(int portalId, DateTime? sinceDate)
        {
            return PlatformDataProvider.Instance().ExecuteReader("Export_GetPortalLocalizations", portalId, sinceDate);
        }

        public IDataReader GetFolders(int portalId, DateTime? sinceDate)
        {
            return PlatformDataProvider.Instance().ExecuteReader("Export_GetFolders", portalId, sinceDate);
        }

        public IDataReader GetFolderPermissionsByPath(int portalId, string folderPath, DateTime? sinceDate)
        {
            return PlatformDataProvider.Instance()
                .ExecuteReader("Export_GetFolderPermissionsByPath", portalId, folderPath, sinceDate);
        }

        public IDataReader GetFolderMappings(int portalId, DateTime? sinceDate)
        {
            return PlatformDataProvider.Instance().ExecuteReader("Export_GetFolderMappings", portalId, sinceDate);
        }

        public IDataReader GetFiles(int portalId, int folderId, DateTime? sinceDate)
        {
            return PlatformDataProvider.Instance().ExecuteReader("Export_GetFiles", portalId, folderId, sinceDate);
        }

        public int? GetPermissionId(string permissionCode, string permissionKey, string permissionName)
        {
            return
                CBO.GetCachedObject<IEnumerable<PermissionInfo>>(new CacheItemArgs(DataCache.PermissionsCacheKey,
                    DataCache.PermissionsCacheTimeout,
                    DataCache.PermissionsCachePriority),
                    c =>
                        CBO.FillCollection<PermissionInfo>(
                            PlatformDataProvider.Instance().ExecuteReader("GetPermissions")))
                    .FirstOrDefault(x => x.PermissionCode == permissionCode &&
                                         x.PermissionKey == permissionKey
                                         && x.PermissionName == permissionName)?.PermissionID;
        }

        #endregion
    }
}