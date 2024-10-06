using System.Globalization;
using System.Text.RegularExpressions;

namespace CardsDataIntegration.Validators;

public class FieldsValidator
{
  public static bool IsValid(string value, string fieldName, string lastName)
  {
    if (string.IsNullOrWhiteSpace(value))
    {
      MessageBox.Show($"Ошибка: {fieldName} не может быть пустым для клиента {lastName}.");
      return false;
    }

    return true;
  }

  public static bool IsValidCardCode(string cardCode, out string errorMessage)
  {
    if (string.IsNullOrWhiteSpace(cardCode))
    {
      errorMessage = "Поле CardCode не должно быть пустым.";
      return false;
    }

    if (!Regex.IsMatch(cardCode, @"^\d+$"))
    {
      errorMessage = "Поле CardCode должно состоять только из цифр.";
      return false;
    }

    errorMessage = null;
    return true;
  }

  public static bool IsDateValid(string birthdayString, out DateTime birthday, string lastName)
  {
    if (!DateTime.TryParseExact(
          birthdayString,
          new[] {"d.M.yyyy", "M.d.yyyy", "yyyy-MM-dd", "dd.MM.yyyy", "d/MM/yyyy", "MM/dd/yyyy", "yyyy/MM/dd"},
          CultureInfo.InvariantCulture, DateTimeStyles.None, out birthday))
    {
      MessageBox.Show($"Неверный формат даты для клиента {lastName}: {birthdayString}");
      return false;
    }

    return true;
  }

  public static bool IsNumericValueValid(string valueString, string lastName, string fieldName, out int? result)
  {
    result = null;
    if (!string.IsNullOrWhiteSpace(valueString))
    {
      if (!int.TryParse(valueString, out int parsedValue))
      {
        MessageBox.Show($"Некорректный формат {fieldName} для клиента {lastName}: {valueString}");
        return false;
      }

      result = parsedValue;
    }

    return true;
  }

  public static bool IsPincodeValid(string pincodeString, string lastName, out int? pincode)
  {
    pincode = null;
    if (!string.IsNullOrWhiteSpace(pincodeString))
    {
      pincodeString = Regex.Replace(pincodeString, @"\D", "");
      return IsNumericValueValid(pincodeString, lastName, "пин-кода", out pincode);
    }

    return true;
  }

  public static bool IsBonusValid(string bonusString, string lastName, out int? bonus)
  {
    return IsNumericValueValid(bonusString, lastName, "бонуса", out bonus);
  }

  public static bool IsTurnoverValid(string turnoverString, string lastName, out int? turnover)
  {
    return IsNumericValueValid(turnoverString, lastName, "оборота", out turnover);
  }
}
