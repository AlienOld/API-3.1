using API_3._1.Resource.Api.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_3._1.Resource.Api.Classes
{
    public class GetUserElements
    {
        public static List<T_sensor> GetUserT_sensors(int user_id, ProjectContext context)
        {
            List<T_sensor> t_Sensors;

            string query = "Select distinct([T_sensors].[Id]), Coordinates, Min_value, Max_value, Is_working, RoomId, Value From T_sensors, Enterprises, Rooms" +
                    $" Where [Enterprises].[UserId] = @user_id and" +
                    $" [Rooms].[EnterpriseId] = [Enterprises].[Id] and" +
                    $" [T_sensors].[RoomId] = [Rooms].[Id]";

            return t_Sensors = context.T_sensors.FromSqlRaw(query, new SqlParameter("user_id", user_id)).ToList();
        }

        public static List<T_sensor> GetUserT_sensorsFromRoom(int room_id, ProjectContext context)
        {
            List<T_sensor> t_Sensors;

            string query = "Select distinct([T_sensors].[Id]), Coordinates, Min_value, Max_value, Is_working, RoomId, Value From T_sensors, Rooms" +
                    $" Where [T_sensors].[RoomId] = @room_id";

            return t_Sensors = context.T_sensors.FromSqlRaw(query, new SqlParameter("room_id", room_id)).ToList();
        }

        public static List<H_sensor> GetUserH_sensors(int user_id, ProjectContext context)
        {
            List<H_sensor> h_Sensors;

            string query = "Select distinct([H_sensors].[Id]), Coordinates, Min_value, Max_value, Is_working, RoomId, Value From H_sensors, Enterprises, Rooms" +
                    $" Where [Enterprises].[UserId] = @user_id and" +
                    $" [Rooms].[EnterpriseId] = [Enterprises].[Id] and" +
                    $" [H_sensors].[RoomId] = [Rooms].[Id]";

            return h_Sensors = context.H_sensors.FromSqlRaw(query, new SqlParameter("user_id", user_id)).ToList();
        }

        public static List<H_sensor> GetUserH_sensorsFromRoom(int room_id, ProjectContext context)
        {
            List<H_sensor> h_Sensors;

            string query = "Select distinct([H_sensors].[Id]), Coordinates, Min_value, Max_value, Is_working, RoomId, Value From H_sensors, Rooms" +
                    $" Where [H_sensors].[RoomId] = @room_id";

            return h_Sensors = context.H_sensors.FromSqlRaw(query, new SqlParameter("room_id", room_id)).ToList();
        }

        public static List<Room> GetUserRooms(int user_id, ProjectContext context)
        {
            List<Room> rooms;

            string query = "Select distinct([Rooms].[Id]), [Rooms].[Name], EnterpriseId, Alarm From Enterprises, Rooms" +
                    $" Where [Enterprises].[UserId] = @user_id and" +
                    $" [Rooms].[EnterpriseId] = [Enterprises].[Id]";

            return rooms = context.Rooms.FromSqlRaw(query, new SqlParameter("user_id", user_id)).ToList();
        }

        public static List<Enterprise> GetUserEnterprises(int user_id, ProjectContext context)
        {
            List<Enterprise> enterprises;

            string query = "Select distinct([Enterprises].[Id]), [Enterprises].[Name], Address, [Enterprises].[UserId] From Enterprises" +
                    $" Where [Enterprises].[UserId] = @user_id";

            return enterprises = context.Enterprises.FromSqlRaw(query, new SqlParameter("user_id", user_id)).ToList();
        }

        public static bool AddNewItem(int user_id, string item, ProjectContext context)
        {
            bool add = false;

            Tariff user_tariff = context.Tariffs.Find(context.Users.Find(user_id).TariffId);

            int user_enterprises = GetUserEnterprises(user_id, context).Count();
            int user_rooms = GetUserRooms(user_id, context).Count();
            int user_sensors = GetUserH_sensors(user_id, context).Count() + GetUserT_sensors(user_id, context).Count();

            switch (item.ToLower())
            {
                case "enterprise":
                    if (user_enterprises < user_tariff.Max_enterprises)
                        add = true;
                    break;
                case "room":
                    if (user_rooms < user_tariff.Max_rooms)
                        add = true;
                    break;
                case "sensor":
                    if (user_sensors < user_tariff.Max_sensors)
                        add = true;
                    break;
            }

            return add;
        }
    }
}













//List<Enterprise> enterprises = _context.Enterprises.Where(x => x.UserId == user.Id).ToList();
//List<Room> rooms = new List<Room>();

//foreach (Enterprise en in enterprises)
//{
//    rooms.AddRange(_context.Rooms.Where(x => x.EnterpriseId == en.Id));
//}

//List<T_sensor> t_Sensors = new List<T_sensor>();

//foreach(Room room in rooms)
//{
//    t_Sensors.AddRange(_context.T_sensors.Where(x => x.RoomId == room.Id));
//}

//if(t_Sensors.Where(x => x.Id == id).ToList().Count == 0)
//{
//    return NotFound();
//} 

