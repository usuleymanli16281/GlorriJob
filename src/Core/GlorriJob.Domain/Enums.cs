using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlorriJob.Domain
{
    public enum VacancyType
    {
        Commission,
        Shift,
        Temporary,
        Volunteer,
        Freelance,
        FullTime,
        InternShip,
        PartTime,
        Permanent,
        Scholarship,
        Seasonal
    }

    public enum JobLevel
    {
        Associate,
        Director,
        EntryLevel,
        Executive,
        Graduate,
        MidLevel,
        Other,
        Professional,
        Student,
        Supervisor
    }

    public enum ResponseStatusCode
    {
        Success = 200,
        Created = 201,
        BadRequest = 400,
        Unauthorized = 401,
        NotFound = 404,
        InternalServerError = 500,
        Conflict = 409,
    }



}
