using System.ComponentModel.DataAnnotations;

namespace ClientDataParser.Domains;

public class Client
{
  [Key]
  public string CardCode { get; set; } = null!; // Код карты клиента
  public string? LastName { get; set; } // Фамилия
  public string? FirstName { get; set; } // Имя
  public string? SurName { get; set; } // Отчество
  public string PhoneMobile { get; set; } = null!; // Мобильный телефон
  public string? Email { get; set; } // Электронная почта
  public string? GenderId { get; set; } // Пол (муж/жен)
  public DateTime Birthday { get; set; } // Дата рождения
  public string? City { get; set; } // Город
  public int? Pincode { get; set; } // Почтовый индекс (nullable, так как может отсутствовать)
  public int? Bonus { get; set; } // Бонус (nullable)
  public int? Turnover { get; set; } // Оборот (nullable)
}
