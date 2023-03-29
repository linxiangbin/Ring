using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Authorization;

namespace Ring.Play.Interface.Controllers
{
    [ApiController]
    [Route("api/Home")]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        [Route("News")]
        public void News(int count)
        {
            try
            {
                SqlConnection connection = new SqlConnection();
                connection.ConnectionString = "Data Source=.\\LINXIANGBINSQL;Initial Catalog=KQWeb;persist security info=True;user id=sa;password=sa123456;";
                connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = "select Number,* from DC_News_Second where N_ID=1";

                SqlDataAdapter dataAdapter = new SqlDataAdapter(command);

                DataSet dataSet = new DataSet();
                var v = dataAdapter.Fill(dataSet);

                var table = dataSet.Tables[0];
                var number = table.Rows[0]["Number"] == null ? 0 : Convert.ToInt32(table.Rows[0]["Number"]);


                SqlCommand commandUpdate = new SqlCommand();
                commandUpdate.Connection = connection;
                int i = number + 1;
                commandUpdate.CommandText = string.Format("update DC_News_Second set Number={0} where N_ID=1", i);
                int result = commandUpdate.ExecuteNonQuery();

                //return "成功";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("Get")]
        public void Get()
        {
            try
            {
                for (int i = 0; i < 10; i++)
                {
                    News(i);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("Proc")]
        public void Proc()
        {
            try
            {
                Parallel.For(0, 1000, (i) =>
                {
                    SqlConnection connection = new SqlConnection();
                    connection.ConnectionString = "Data Source=.\\LINXIANGBINSQL;Initial Catalog=KQWeb;persist security info=True;user id=sa;password=sa123456;";
                    connection.Open();

                    SqlCommand command = new SqlCommand();
                    command.Connection = connection;
                    command.CommandText = "News_Loop_Test";
                    command.CommandType = CommandType.StoredProcedure;

                    int result = command.ExecuteNonQuery();

                    connection.Close();
                });
            }
            catch (Exception ex)
            {

            }
        }

        [Authorize]
        [HttpGet]
        [Route("GetParallel")]
        public string GetParallel()
        {
            try
            {
                //Action<int> action = News;
                //Parallel.For(10, 1000, Proc);

                for (int i = 0; i < 100; i++)
                {
                    Add(i);
                }
                return "成功";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private void Add(int i)
        {
            try
            {
                SqlConnection connection = new SqlConnection();
                connection.ConnectionString = "Data Source=.\\LINXIANGBINSQL;Initial Catalog=KQWeb;persist security info=True;user id=sa;password=sa123456;";
                connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = "News_Loop_Test";
                command.CommandType = CommandType.StoredProcedure;

                int result = command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {

            }
        }

    }
}
