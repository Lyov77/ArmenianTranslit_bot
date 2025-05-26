using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

public class BotService
{
    private readonly ITelegramBotClient _botClient;
    private readonly LanguageManager _languageManager = new();

    public BotService(ITelegramBotClient botClient)
    {
        _botClient = botClient;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
    {
        Console.WriteLine("Got a message!");
        if (update.Type == UpdateType.CallbackQuery)
        {
            var lang = update.CallbackQuery.Data!;
            var user_Id = update.CallbackQuery.From.Id;

            _languageManager.SetLanguage(user_Id, lang);

            string welcome = lang switch
            {
                "hy" => "Բարև, ես Telegram բոտ եմ, որը փոխակերպում է լատինատառ հայերենը՝ հայերեն այբուբենով։\nՕրինակ, ուղարկիր `barev`, ես կպատասխանեմ՝ `բարև`։",
                "ru" => "Привет! Я Telegram-бот, который превращает армянский текст, написанный латиницей, в армянский алфавит.\nНапример, отправь: `barev`, я отвечу: `բարև`.",
                "en" => "Hello! I'm a Telegram bot that converts Armenian written in Latin letters into the Armenian alphabet.\nFor example, send: `barev` and I’ll reply: `բարև`.",
                _ => "Language not recognized."
            };

            await bot.AnswerCallbackQuery(update.CallbackQuery.Id);
            await bot.SendMessage(update.CallbackQuery.Message.Chat.Id, welcome, cancellationToken: cancellationToken);
            return;
        }

        if (update.Type != UpdateType.Message || update.Message!.Type != MessageType.Text)
            return;

        var message = update.Message;
        var text = message.Text!.Trim();
        var userId = message.From!.Id;

        if (text == "/start" || text == "/language")
        {
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new[] {
                    InlineKeyboardButton.WithCallbackData("🇦🇲 Հայերեն", "hy"),
                    InlineKeyboardButton.WithCallbackData("🇷🇺 Русский", "ru"),
                    InlineKeyboardButton.WithCallbackData("🇬🇧 English", "en")
                }
            });

            await bot.SendMessage(message.Chat.Id,
                "Please select your language / Խնդրում եմ ընտրեք լեզուն / Пожалуйста, выберите язык:",
                replyMarkup: keyboard,
                cancellationToken: cancellationToken);
        }
        else
        {
            var lang = _languageManager.GetLanguage(userId) ?? "en"; // default to English
            var transliterated = Transliterator.Transliterate(text);

            string prefix = lang switch
            {
                "hy" => "",
                "ru" => "",
                "en" => "",
                _ => ""
            };

            await bot.SendMessage(message.Chat.Id, $"{prefix}{transliterated}", cancellationToken: cancellationToken);
        }
    }

    public Task HandleErrorAsync(ITelegramBotClient bot, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Error: {exception.Message}");
        return Task.CompletedTask;
    }
}
