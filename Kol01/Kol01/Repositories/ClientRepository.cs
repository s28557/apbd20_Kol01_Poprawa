using Kol01.Models;
using Microsoft.Data.SqlClient;

namespace Kol01.Repositories;

public class ClientRepository : IClientRepository
{
    private readonly IConfiguration _configuration;

    public ClientRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<bool> DoesCarExist(int id)
    {
        var query = "SELECT 1 FROM cars WHERE ID = @ID";

        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", id);

        await connection.OpenAsync();

        var res = await command.ExecuteScalarAsync();

        return res is not null;
    }

    public async Task<bool> DoesClientExist(int id)
    {
        var query = "SELECT 1 FROM clients WHERE ID = @ID";

        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", id);

        await connection.OpenAsync();

        var res = await command.ExecuteScalarAsync();

        return res is not null;
    }

    public async Task<Client> GetClient(int id)
    {
        var query = @"SELECT clients.ID, clients.FirstName, clients.LastName, clients.Address, cars.VIN, 
                        colors.Name AS color, models.Name AS model, 
                        car_rentals.DateFrom, car_rentals.DateTo, car_rentals.TotalPrice
                        from clients 
                        join car_rentals ON clients.ID = car_rentals.ClientID
                        join cars ON car_rentals.CarID = car.ID
                        join models ON cars.ModelID = models.ID
                        join colors ON cars.ColorID = colors.ID
                        where clients.ID = @ID";
        
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", id);

        await connection.OpenAsync();

        var reader = await command.ExecuteReaderAsync();

        Client client = null;

        while (await reader.ReadAsync())
        {
            if (client == null)
            {
                client = new Client
                {
                    ID = reader.GetInt32(reader.GetOrdinal("ID")),
                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                    Address = reader.GetString(reader.GetOrdinal("Address")),

                    Rentals = new List<RentalDto>
                    {
                        new RentalDto
                        {
                            Vin = reader.GetString(reader.GetOrdinal("VIN")),
                            Color = reader.GetString(reader.GetOrdinal("Color")),
                            Model = reader.GetString(reader.GetOrdinal("Model")),
                            DateFrom = reader.GetDateTime(reader.GetOrdinal("DateFrom")),
                            DateTo = reader.GetDateTime(reader.GetOrdinal("DateTo")),
                            TotalPrice = reader.GetInt32(reader.GetOrdinal("TotalPrice"))
                        }
                    }
                };
            }
        }
        return client;
    }

    public async Task<int> AddNewClient(NewClientDto newClientDto)
    {
        var insertClient = @"INSERT INTO clients VALUES(@FirstName, @LastName, @Address);
                        SELECT @@IDENTITY AS ID;";
        
        var insertRental = @"INSERT INTO car_rentals 
                            VALUES (@ClientID, @CarID, @DateFrom, @DateTo, @TotalPrice, @Discount)";

        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();
        await using SqlCommand command2 = new SqlCommand();

        command.Connection = connection;
        command.CommandText = insertClient;
        command.Parameters.AddWithValue("@FirstName", newClientDto.FirstName);
        command.Parameters.AddWithValue("@LastName", newClientDto.LastName);
        command.Parameters.AddWithValue("@Address", newClientDto.Address);
        command2.Parameters.AddWithValue("@CarID", CarRentalDto.CarID);
        command2.Parameters.AddWithValue("@DateFrom", CarRentalDto.DateFrom);
        command2.Parameters.AddWithValue("@DateTo", CarRentalDto.DateTo);
        
        await connection.OpenAsync();
        var transaction = await connection.BeginTransactionAsync();
        command.Transaction = transaction as SqlTransaction;
        return 1;
    }
}