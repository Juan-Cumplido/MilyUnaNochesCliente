using MilyUnaNochesWPFApp.MilyUnaNochesProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilyUnaNochesWPFApp.Logic
{
    internal class UserProfileSingleton
    {

        private static readonly UserProfileSingleton _singletonInstance = new UserProfileSingleton();
        public static int idAcceso { get; set; }
        public static int idEmployee { get; set; }
        public static string typeEmployee { get; set; }
        public static string name { get; set; }
        public static string firstLastName { get; set; }
        public static string secondLastName { get; set; }
        public static string email { get; set; }
        public static string phone { get; set; }

        public static UserProfileSingleton Instance => _singletonInstance;

        public void CreateInstance(Empleado employee)
        {
            idAcceso = employee.idAcceso;
            idEmployee = employee.idEmpleado;
            typeEmployee = employee.tipoEmpleado;
            name = employee.nombre;
            firstLastName = employee.primerApellido;
            secondLastName = employee.segundoApellido;
            email = employee.correo;
            phone = employee.telefono;
        }

        public void ResetSingleton()
        {
            idAcceso = 0;
            idEmployee = 0;
            typeEmployee = null;
            name = null;
            firstLastName = null;
            secondLastName = null;
            email = null;
            phone = null;
        }
    }
}
