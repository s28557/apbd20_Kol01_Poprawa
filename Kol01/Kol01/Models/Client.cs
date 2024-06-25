using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;

namespace Kol01.Models;


public class Client
{
    public int ID { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Address { get; set; }
    public List<RentalDto> Rentals { get; set; }
}
public class CarRentalDto
{
    public static int ID { get; set; }
    public static int ClientID { get; set; }
    public static int CarID { get; set; }
    public static DateTime DateFrom { get; set; }
    public static DateTime DateTo { get; set; }
    public static int TotalPrice { get; set; }
    //public int Discount { get; set; }
}

public class NewClientDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Address { get; set; }
    public CarRentalDto CarRentalDto { get; set; }
    
    
}

