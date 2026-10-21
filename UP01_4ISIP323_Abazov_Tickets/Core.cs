using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace UP01_4ISIP323_Abazov_Tickets
{
    public class Core
    {
        public static DB_Abazov_TicketsEntities3 Context = new DB_Abazov_TicketsEntities3();

        public static string GetHash(string password)
        {
            using (var hash = SHA1.Create())
            {
                return string.Concat(hash.ComputeHash(Encoding.UTF8.GetBytes(password)).Select(x => x.ToString("X2")));
            }
        }

        public static bool ValidatePassword(string password, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (string.IsNullOrEmpty(password) || password.Length < 6)
            {
                errorMessage = "Пароль слишком короткий, должно быть минимум 6 символов!";
                return false;
            }

            bool en = true;
            bool number = false;

            for (int i = 0; i < password.Length; i++)
            {
                if (password[i] >= '0' && password[i] <= '9')
                    number = true;
                else if (!((password[i] >= 'A' && password[i] <= 'Z') || (password[i] >= 'a' && password[i] <= 'z')))
                    en = false;
            }

            if (!en)
            {
                errorMessage = "Используйте только английскую раскладку!";
                return false;
            }
            if (!number)
            {
                errorMessage = "Добавьте хотя бы одну цифру!";
                return false;
            }

            return true;
        }

        public static string GenerateCaptcha()
        {
            string allowchar = "A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z,a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,y,z,1,2,3,4,5,6,7,8,9,0";
            string[] ar = allowchar.Split(',');
            string pwd = "";
            Random r = new Random();
            for (int i = 0; i < 6; i++)
            {
                pwd += ar[r.Next(0, ar.Length)];
            }
            return pwd;
        }

        public static void ShowInfo(string message)
        {
            MessageBox.Show(message, "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void ShowError(string message)
        {
            MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}