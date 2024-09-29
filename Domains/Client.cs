
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CardsDataIntegration.Domains;

public class Client
{
  //[NotMapped]
  //public int Index { get; set; } // Для сохранения порядка, не сохраняется в БД
  
  
  
  [Key]
  public string CardCode { get; set; } // Код карты клиента
  public string LastName { get; set; } // Фамилия
  public string FirstName { get; set; } // Имя
  public string SurName { get; set; } // Отчество
  public string PhoneMobile { get; set; } // Мобильный телефон
  public string Email { get; set; } // Электронная почта
  public string GenderId { get; set; } // Пол (муж/жен)
  public DateTime Birthday { get; set; } // Дата рождения
  public string City { get; set; } // Город
  public int? Pincode { get; set; } // Почтовый индекс (nullable, так как может отсутствовать)
  public int? Bonus { get; set; } // Бонус (nullable)
  public decimal? Turnover { get; set; } // Оборот (nullable)
  
  //public int InsertionOrder { get; set; } // Добавьте это поле
}
