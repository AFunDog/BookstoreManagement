using System.Collections.Frozen;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.Json;
using CoreLibrary.Toolkit.Extensions;
using Mapster;
using Serilog;
using 书店管理系统.Core;
using 书店管理系统.Core.Structs;

namespace 书店管理系统.ConsoleApp
{
    internal class Program
    {
        static readonly IReadOnlyDictionary<string, Func<string[], Task>> _commands = new Dictionary<string, Func<string[], Task>>
        {
            ["exit"] = ExitAsync,
            ["admin"] = AdminLoginAsync,
            ["login"] = LoginAsync,
            ["logout"] = LogoutAsync,
            ["buy"] = BuyBookAsync,
            ["recharge"] = RechargeAsync,
            ["pass"] = PassRechargeRequestAsync,
            ["edit"] = EditDataAsync,
            ["add"] = AddDataAsync,
            ["remove"] = RemoveDataAsync,
            ["show"] = ShowDataAsync,
            ["my"] = ShowMyUserDataAsync,
        }.ToFrozenDictionary();

        static bool IsRunning { get; set; } = true;

        static async Task Main(string[] args)
        {
            await LibrarySystemManager.InitAsync();
            Output.Info($"书店管理系统初始化完毕\n当前版本{LibrarySystemManager.Version}");
#if DEBUG
            Output.Info("当前为Debug模式");
#endif
            while (IsRunning && (Console.ReadLine() ?? "").Split(' ') is string[] commands)
            {
                if (_commands.TryGetValue(commands[0], out var func))
                {
                    try
                    {
                        await func(commands[1..]);
                    }
                    catch (Exception e)
                    {
#if DEBUG
                        Output.Error("{0}", e);
#else
                        Output.Error("{0}", e.Message);
#endif
                    }
                }
                else
                {
                    Output.Error("未知命令，请重新输入");
                }
            }

            LibrarySystemManager.ExitSystem();
            Output.Info("书店管理系统已退出");
        }

        internal static async Task AdminLoginAsync(string[] arg)
        {
            string password = ArgsConverter.Convert<string>(arg);
            await LibrarySystemManager.Instance.UserService.LoginAsAdminAsync(password);
            Output.Info("管理员登录成功");
        }

        internal static async Task LoginAsync(string[] args)
        {
            (string userName, string password) = ArgsConverter.Convert<string, string>(args);
            await LibrarySystemManager.Instance.UserService.LoginAsUserAsync(userName, password);
            Output.Info($"用户{LibrarySystemManager.Instance.UserService.LoginUser!.Name}登录成功");
        }

        internal static async Task LogoutAsync(string[] args)
        {
            await LibrarySystemManager.Instance.UserService.LogoutAsync();
            Output.Info("退出登录成功");
        }

        internal static async Task EditDataAsync(string[] args)
        {
            string target = ArgsConverter.Convert<string>(args);
            switch (target)
            {
                case "user":
                    await EditUserDataAsync(ArgsConverter.Convert<int>(args[1..]), args[2..]);
                    Output.Info("修改用户数据成功");
                    break;
                case "book":
                    await EditBookDataAsync(ArgsConverter.Convert<int>(args[1..]), args[2..]);
                    Output.Info("修改书籍数据成功");
                    break;
                default:
                    Output.Error("未知数据目标");
                    break;
            }
        }

        internal static async Task AddDataAsync(string[] args)
        {
            string target = ArgsConverter.Convert<string>(args).ToLower();
            switch (target)
            {
                case "user":
                    await AddUserDataAsync(args[1..]);
                    Output.Info("添加用户数据成功");
                    break;
                case "book":
                    await AddBookDataAsync(args[1..]);
                    Output.Info("添加书籍数据成功");
                    break;
                default:
                    Output.Error("未知数据目标");
                    break;
            }
        }

        internal static async Task RemoveDataAsync(string[] args)
        {
            string target = ArgsConverter.Convert<string>(args).ToLower();
            switch (target)
            {
                case "user":
                    await RemoveUserDataAsync(ArgsConverter.Convert<int>(args[1..]));
                    Output.Info("移除用户数据成功");
                    break;
                case "book":
                    await RemoveBookDataAsync(ArgsConverter.Convert<int>(args[1..]));
                    Output.Info("移除书籍数据成功");
                    break;
                case "bookdeal":
                    await RemoveBookDealDataAsync(ArgsConverter.Convert<int>(args[1..]));
                    Output.Info("移除书籍交易数据成功");
                    break;
                case "rechargedeal":
                    await RemoveRechargeDealDataAsync(ArgsConverter.Convert<int>(args[1..]));
                    Output.Info("移除充值交易数据成功");
                    break;
                default:
                    Output.Error("未知数据目标");
                    break;
            }
        }

        internal static async Task ShowDataAsync(string[] args)
        {
            string target = ArgsConverter.Convert<string>(args).ToLower();
            switch (target)
            {
                case "user":
                    await ShowUserDataAsync();
                    break;
                case "book":
                    await ShowBookDataAsync();
                    break;
                case "bookdeal":
                    await ShowBookDealDataAsync();
                    break;
                case "rechargedeal":
                    await ShowRechargeDealDataAsync();
                    break;
                default:
                    break;
            }
        }

        internal static async Task ShowMyUserDataAsync(string[] args)
        {
            if (LibrarySystemManager.Instance.UserService.LoginUser is UserData user)
            {
                Output.Info("当前用户信息：");
                Output.Info(
                    "Id : {0} Name : {1} Password : {2} Gender : {3} Phone : {4} Address : {5} Email : {6} CreateTime : {7} UpdateTime : {8} Account : {9}",
                    user.Id,
                    user.Name,
                    user.Password,
                    user.Gender,
                    user.Phone,
                    user.Address,
                    user.Email,
                    user.CreateTime,
                    user.UpdateTime,
                    user.Account
                );
            }
            else
            {
                Output.Error("未登录无法查看本用户信息，请先登录");
            }
        }

        internal static async Task BuyBookAsync(string[] args)
        {
            (long ISBN, int count) = ArgsConverter.Convert<long, int>(args);
            await LibrarySystemManager.Instance.BookService.BuyBookAsync(ISBN, count);
            var book = LibrarySystemManager.Instance.BookService.BookDatas.First(b => b.ISBN == ISBN);
            Output.Info("成功购买书籍 {0} 共 {1} 本 总价格 {2}", book.BookName, count, book.Price * count);
        }

        internal static async Task RechargeAsync(string[] args)
        {
            (int targetUid, decimal addMoney) = ArgsConverter.Convert<int, decimal>(args);
            await LibrarySystemManager.Instance.DealService.AddNewRechargeDealAsync(targetUid, addMoney);
            Output.Info("充值申请成功，请等待管理员审批通过");
        }

        internal static async Task PassRechargeRequestAsync(string[] args)
        {
            int id = ArgsConverter.Convert<int>(args);
            await LibrarySystemManager.Instance.DealService.PassRechargeDealAsync(id);
            Output.Info("编号为 {0} 的充值申请已通过", id);
        }

        #region UserData

        private static async Task AddUserDataAsync(string[] properties)
        {
            var propertyValues = PropertiesToPropertyValues(properties);
            string name = propertyValues["name"];
            string password = propertyValues["password"];
            Gender gender = propertyValues.TryGetValue("gender", out var genderStr)
                ? genderStr switch
                {
                    "male" or "男" => Gender.Male,
                    "female" or "女" => Gender.Female,
                    "other" or "其他" => Gender.Other,
                    _ => Gender.Unknown,
                }
                : Gender.Unknown;
            string phone = propertyValues.TryGetValue("phone", out var phoneStr) ? phoneStr : string.Empty;
            string address = propertyValues.TryGetValue("address", out var addressStr) ? addressStr : string.Empty;
            string email = propertyValues.TryGetValue("email", out var emailStr) ? emailStr : string.Empty;
            await LibrarySystemManager.Instance.UserService.RegisterUserAsync(name, password, gender, phone, address, email);
        }

        private static async Task EditUserDataAsync(int id, string[] properties)
        {
            var propertyValues = PropertiesToPropertyValues(properties);
            string? name = propertyValues.TryGetValue("name", out var nameStr) ? nameStr : null;
            string? password = propertyValues.TryGetValue("password", out var passwordStr) ? passwordStr : null;
            Gender? gender = propertyValues.TryGetValue("gender", out var genderStr)
                ? genderStr switch
                {
                    "male" or "男" => Gender.Male,
                    "female" or "女" => Gender.Female,
                    "other" or "其他" => Gender.Other,
                    "unknown" or "未知" => Gender.Unknown,
                    _ => null,
                }
                : null;
            string? phone = propertyValues.TryGetValue("phone", out var phoneStr) ? phoneStr : null;
            string? address = propertyValues.TryGetValue("address", out var addressStr) ? addressStr : null;
            string? email = propertyValues.TryGetValue("email", out var emailStr) ? emailStr : null;
            await LibrarySystemManager.Instance.UserService.EditUserBasicDataAsync(id, name, password, gender, phone, address, email);
        }

        private static async Task RemoveUserDataAsync(int id)
        {
            await LibrarySystemManager.Instance.UserService.RemoveUserAsync(id);
        }

        private static async Task ShowUserDataAsync()
        {
            Output.Info("用户数据数量：{0}", LibrarySystemManager.Instance.UserService.UserDatas.Count);
            foreach (var user in LibrarySystemManager.Instance.UserService.UserDatas)
            {
                Output.Info(
                    "Id : {0} Name : {1} Password : {2} Gender : {3} Phone : {4} Address : {5} Email : {6} CreateTime : {7} UpdateTime : {8} Account : {9}",
                    user.Id,
                    user.Name,
                    user.Password,
                    user.Gender,
                    user.Phone,
                    user.Address,
                    user.Email,
                    user.CreateTime,
                    user.UpdateTime,
                    user.Account
                );
            }
        }

        #endregion

        #region BookData

        private static async Task AddBookDataAsync(string[] properties)
        {
            var propertyValues = PropertiesToPropertyValues(properties);
            long ISBN = int.Parse(propertyValues["isbn"]);
            string bookName = propertyValues["bookname"];
            string author = propertyValues["author"];
            string publisher = propertyValues["publisher"];
            DateTime publicationDate = DateTime.Parse(propertyValues["publicationdate"]);
            string[] category = [];
            string description = propertyValues["description"];
            decimal price = decimal.Parse(propertyValues["price"]);
            int amount = int.Parse(propertyValues["amount"]);
            await LibrarySystemManager.Instance.BookService.AddBookAsync(
                new(ISBN, bookName, author, publisher, publicationDate, category, description, price, amount)
            );
        }

        private static async Task EditBookDataAsync(long ISBN, string[] properties)
        {
            var propertyValues = PropertiesToPropertyValues(properties);
            string? bookName = propertyValues.TryGetValue("bookname", out var bookNameStr) ? bookNameStr : null;
            string? author = propertyValues.TryGetValue("author", out var authorStr) ? authorStr : null;
            string? publisher = propertyValues.TryGetValue("publisher", out var publisherStr) ? publisherStr : null;
            DateTime? publicationDate = propertyValues.TryGetValue("publishtdate", out var publicationDateStr)
                ? DateTime.Parse(publicationDateStr)
                : null;
            string[]? category = null;
            string? description = propertyValues.TryGetValue("description", out var descriptionStr) ? descriptionStr : null;
            decimal? price = propertyValues.TryGetValue("price", out var priceStr) ? decimal.Parse(priceStr) : null;
            int? addAmount = propertyValues.TryGetValue("amount", out var addAmountStr) ? int.Parse(addAmountStr) : null;
            await LibrarySystemManager.Instance.BookService.EditBookDataAsync(
                ISBN,
                bookName,
                author,
                publisher,
                publicationDate,
                category,
                description
            );
            if (price is not null)
            {
                await LibrarySystemManager.Instance.BookService.ChangeBookPriceAsync(ISBN, price.Value);
            }
            if (addAmount is not null)
            {
                await LibrarySystemManager.Instance.BookService.SupplyBookAsync(ISBN, addAmount.Value);
            }
        }

        private static async Task RemoveBookDataAsync(int id)
        {
            await LibrarySystemManager.Instance.BookService.RemoveBookAsync(id);
        }

        private static async Task ShowBookDataAsync()
        {
            Output.Info("书籍数据数量：{0}", LibrarySystemManager.Instance.BookService.BookDatas.Count);
            foreach (var book in LibrarySystemManager.Instance.BookService.BookDatas)
            {
                Output.Info(
                    "ISBN : {0} BookName : {1} Author : {2} Publisher : {3} PublicationDate : {4} Category : {5} Description : {6} Price : {7} Amount : {8}",
                    book.ISBN,
                    book.BookName,
                    book.Author,
                    book.Publisher,
                    book.PublicationDate,
                    book.Category,
                    book.Description,
                    book.Price,
                    book.Amount
                );
            }
        }

        #endregion

        #region BookDealData
        private static async Task RemoveBookDealDataAsync(int id)
        {
            await LibrarySystemManager.Instance.DealService.RemoveBookDealAsync(id);
        }

        private static async Task ShowBookDealDataAsync()
        {
            Output.Info("书籍交易数据数量：{0}", LibrarySystemManager.Instance.DealService.BookDealDatas.Count);
            foreach (var deal in LibrarySystemManager.Instance.DealService.BookDealDatas)
            {
                Output.Info(
                    "Id : {0} Uid : {1} ISBN : {2} DealTime : {3} Price : {4} Amount : {5} : TotalPrice : {6}",
                    deal.Id,
                    deal.Uid,
                    deal.ISBN,
                    deal.DealTime,
                    deal.Price,
                    deal.Amount,
                    deal.Amount * deal.Price
                );
            }
        }

        #endregion

        #region RechargeDealData

        private static async Task RemoveRechargeDealDataAsync(int id)
        {
            await LibrarySystemManager.Instance.DealService.RemoveRechargeDealAsync(id);
        }

        private static async Task ShowRechargeDealDataAsync()
        {
            Output.Info("充值交易数据数量：{0}", LibrarySystemManager.Instance.DealService.RechargeDealDatas.Count);
            foreach (var deal in LibrarySystemManager.Instance.DealService.RechargeDealDatas)
            {
                Output.Info(
                    "Id : {0} Uid : {1} AddMoney : {2} DealCreateTime : {3} DealPassTime : {4} IsPass : {5}",
                    deal.Id,
                    deal.Uid,
                    deal.AddMoney,
                    deal.DealCreateTime,
                    deal.DealPassTime == default ? "---" : deal.DealPassTime,
                    deal.IsPass
                );
            }
        }

        #endregion

        internal static async Task ExitAsync(string[] args)
        {
            LibrarySystemManager.ExitSystem();
            IsRunning = false;
        }

        #region Helpers

        private static Dictionary<string, string> PropertiesToPropertyValues(string[] properties)
        {
            return properties
                .Select(pv =>
                {
                    var temp = pv.Split('=', 2);
                    return new KeyValuePair<string, string>(temp[0].Trim().ToLower(), temp[1].Trim().ToLower());
                })
                .ToDictionary();
        }

        #endregion
    }
}
