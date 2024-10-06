using System.Text.RegularExpressions;

namespace ClientDataParser.Validators;

public class PhoneValidator
{
  public static bool IsValid(ref string phoneMobile, string lastName)
  {
    // Сохраняем оригинальный номер для отладки
    string originalPhone = phoneMobile;

    // Убираем все символы, кроме цифр
    string cleanedPhone = Regex.Replace(phoneMobile, @"[^\d]", "");

    // Проверяем длину очищенного номера
    if (cleanedPhone.Length == 11 && cleanedPhone.StartsWith("7"))
    {
      cleanedPhone = "+" + cleanedPhone; // Добавляем '+' в начале
    }
    else if (cleanedPhone.Length == 10)
    {
      cleanedPhone = "+7" + cleanedPhone; // Добавляем '+7' для 10-значного номера
    }
    else
    {
      MessageBox.Show($"Некорректный формат телефона для клиента {lastName}: {originalPhone}");
      return false;
    }

    // Проверяем окончательный формат
    if (!Regex.IsMatch(cleanedPhone, @"^\+7\d{10}$"))
    {
      MessageBox.Show($"Некорректный формат телефона для клиента {lastName}: {cleanedPhone}");
      return false;
    }

    // Присваиваем отформатированный номер обратно
    phoneMobile = cleanedPhone;

    return true;
  } 
}

