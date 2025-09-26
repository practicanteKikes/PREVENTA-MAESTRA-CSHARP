namespace MainProject.Controllers.CustomJwt.Models.CustomJwtModule
{
    public class UserJwtModel
    {
        public string? Id { get; set; }
        public string? User { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        public string? Rol { get; set; }

        public static List<UserJwtModel> DB()
        {
            var list = new List<UserJwtModel>()
            {
                new UserJwtModel
                {
                    Id = "-1",
                    User = "masterjj24s5",
                    Password = "12May2010",
                    Email = "masterjj31@gmail.com",
                    Rol = "developer"
                },
                new UserJwtModel
                {
                    Id = "24",
                    User = "masterjj24",
                    Password = "12May2010",
                    Email = "masterjj31@gmail.com",
                    Rol = "developer"
                },
                new UserJwtModel
                {
                    Id = "25",
                    User = "jahir.higuera",
                    Password = "12May2010",
                    Email = "masterjj31@gmail.com",
                    Rol = "administrador"
                },
                new UserJwtModel
                {
                    Id = "1",
                    User = "example_user_1",
                    Password = "12May2010",
                    Email = "masterjj31@gmail.com",
                    Rol = "empleado"
                },
                new UserJwtModel
                {
                    Id = "2",
                    User = "example_user_2",
                    Password = "12May2010",
                    Email = "masterjj31@gmail.com",
                    Rol = "asesor"
                },
                new UserJwtModel
                {
                    // [token] - eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJXZWJBcGlUdWxpUGFuUHVyY2hhc2VPcmRlcnMiLCJqdGkiOiI5ZGYwMWEyOS00YjBlLTRjNGQtOGU5Ny0wZTYyMjJkYzI2MmMiLCJpYXQiOiIwMi8wOC8yMDIzIDAxOjM0OjA1IHAuwqBtLiIsImlkIjoiMyIsImVtYWlsIjoibWFzdGVyamozMUBnbWFpbC5jb20iLCJpc3MiOiJodHRwczovL3d3dy5odWV2b3NraWtlcy5jb20iLCJhdWQiOiJodHRwczovL3d3dy5ub3ZvY29tcHJhc3R1bGlwYW4uY29tIn0.pqAHjQvwIlTs3BKAPB1YpWPstNnRLuHiayVZ_cGZHUU
                    Id = "3",
                    User = "NCTU",
                    Password = "novocomprastulipan",
                    Email = "masterjj31@gmail.com",
                    Rol = "administrador"

                }
            };

            return list;
        }
    }
}
