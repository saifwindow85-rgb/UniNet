using Contracts.Common.AuthorizationInfos;
using Contracts.Common.AuthorizationInfos.AcademicInfos;
using Contracts.Common.AuthorizationInfos.EmployeeAuthorizationInfo;
using Contracts.Common.AuthorizationInfos.StudentAuthorizationInfo;
using Contracts.Common.AuthorizationInfos.StudyAuthorizationInfos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Common.Mappers
{
    public static class GeneralAuthorizationInfoMapper
    {

        public static GeneralAuthorizationInfo ToStudentInfo(this StudentAuthorizationInfo studentAuthorizationInfo)
        {
            return new GeneralAuthorizationInfo
            {
                UniversityId = studentAuthorizationInfo.UniversityId,
                CollegeId = studentAuthorizationInfo.CollegeId,
                DepartmentId = studentAuthorizationInfo.DepartmentId,
                BatchId = studentAuthorizationInfo.BatchId,
            };
        }

        public static GeneralAuthorizationInfo ToEmployeeInfo(this EmployeeAuthorizationInfo employeeAuthorizationInfo)
        {
            return new GeneralAuthorizationInfo
            {
                UniversityId = employeeAuthorizationInfo.UniversityId,
                CollegeId = employeeAuthorizationInfo.CollegeId,
                DepartmentId = employeeAuthorizationInfo.DepartmentId,

            };
        }
        public static GeneralAuthorizationInfo ToUniversityInfo(this UniversityAuthorizationInfo universityAuthorizationInfo)
        {
            return new GeneralAuthorizationInfo
            {
                UniversityId = universityAuthorizationInfo.UniversityId,
            };
        }
        public static GeneralAuthorizationInfo ToCollegeInfo(this CollegeAuthorizationInfo authorizationInfo)
        {
            return new GeneralAuthorizationInfo
            {
                UniversityId = authorizationInfo.UniversityId,
                CollegeId = authorizationInfo.CollegeId,
            };
        }

        public static GeneralAuthorizationInfo ToDepartmentInfo(this DepartmentAuthorizationInfo authorizationInfo)
        {
            return new GeneralAuthorizationInfo
            {
                UniversityId = authorizationInfo.UniversityId,
                CollegeId = authorizationInfo.CollegeId,
                DepartmentId = authorizationInfo.DepartmentId,
            };

        }


        public static GeneralAuthorizationInfo ToBatchInfo(this BatchAuthorizationInfo authorizationInfo)
        {
            return new GeneralAuthorizationInfo
            {
                UniversityId = authorizationInfo.UniversityId,
                CollegeId = authorizationInfo.CollegeId,
                DepartmentId = authorizationInfo.DepartmentId,
                BatchId = authorizationInfo.BatchId,
            };
        }

        public static GeneralAuthorizationInfo ToSectionInfo(this SectionAuthorizationInfo authorizationInfo)
        {
            return new GeneralAuthorizationInfo
            {
                UniversityId = authorizationInfo.UniversityId,
                CollegeId = authorizationInfo.CollegeId,
                DepartmentId = authorizationInfo.DepartmentId,
                BatchId = authorizationInfo.BatchId,
                SectionId = authorizationInfo.SectionId,
            };
        }

        public static GeneralAuthorizationInfo ToSubjectInfo(this SubjectAuthorizationInfo authorizationInfo)
        {
            return new GeneralAuthorizationInfo
            {
                UniversityId = authorizationInfo.UniversityId,
                CollegeId = authorizationInfo.CollegeId,
                DepartmentId = authorizationInfo.DepartmentId,
            };
        }

        public static GeneralAuthorizationInfo ToSemesterInfo(this SemesterAuthorizationInfo authorizationInfo)
        {
            return new GeneralAuthorizationInfo
            {
                UniversityId = authorizationInfo.UniversityId,
            };
        }
    }
}
