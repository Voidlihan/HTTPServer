using System;
using System.Net;
using System.Text;
using System.Text.Json;

namespace HTTPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var listener = new HttpListener())
            {
                listener.Prefixes.Add("http://localhost/");
                listener.Start();
                while (true)
                {
                    var listenerContext = listener.GetContext();
                    var response = listenerContext.Request;
                    byte[] bytes = new byte[2048];
                    response.InputStream.Read(bytes, 0, bytes.Length);
                    var resRequest = Encoding.UTF8.GetString(bytes);
                    string[] resultText = resRequest.Split(new char[] { '*' });
                    var user = JsonSerializer.Deserialize<User>(resultText[1]);
                    var query = JsonSerializer.Deserialize<Query>(resultText[0]);
                    if (query.QueryType == "AUTH")
                    {
                        Console.WriteLine("Пользователь заходит...");
                        using (var context = new Context())
                        {
                            bool isAuth = false;
                            foreach (var item in context.Users)
                            {
                                if (item.Login == user.Login && item.Password == user.Password)
                                {
                                    isAuth = true;
                                }
                            }
                            if (isAuth)
                            {
                                Console.WriteLine($"{user.Login} зашел!");
                                using (var stream = listenerContext.Response)
                                {
                                    byte[] info = Encoding.UTF8.GetBytes($"Добро пожаловать, {user.Login}!");
                                    stream.OutputStream.Write(info, 0, info.Length);
                                }
                            }
                            else
                            {
                                Console.WriteLine("Пользователь не смог зайти...");
                                using (var stream = listenerContext.Response)
                                {
                                    byte[] info = Encoding.UTF8.GetBytes("Неправильный ввод логина или пароля!");
                                    stream.OutputStream.Write(info, 0, info.Length);
                                }
                            }
                        }
                    }
                    else if (query.QueryType == "SIGNUP")
                    {
                        Console.WriteLine("Пользователь заходит...");
                        using (var contextDb = new Context())
                        {
                            bool isLoginReg = false;
                            foreach (var item in contextDb.Users)
                            {
                                if (item.Login == user.Login)
                                {
                                    isLoginReg = true;

                                }
                            }
                            if (!isLoginReg)
                            {
                                Console.WriteLine($"{user.Login} зарегистрировался!");
                                using (var stream = listenerContext.Response)
                                {
                                    byte[] info = Encoding.UTF8.GetBytes($"Добро пожаловать, {user.Login}!");
                                    stream.OutputStream.Write(info, 0, info.Length);
                                    contextDb.Users.Add(user);
                                    contextDb.SaveChanges();
                                }
                            }
                            else
                            {
                                Console.WriteLine($"Пользователь не смог зарегистрироваться...");
                                using (var stream = listenerContext.Response)
                                {
                                    byte[] info = Encoding.UTF8.GetBytes($"Этот логин недоступен!");
                                    stream.OutputStream.Write(info, 0, info.Length);
                                }
                            }
                        }
                    }

                }
            }
        }
    }
}
