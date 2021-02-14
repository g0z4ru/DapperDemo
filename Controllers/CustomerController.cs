using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DapperDemo.Models;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using System.Data.Common;

namespace DapperDemo.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ILogger<CustomerController> _logger;
        private readonly string _connectionString = "Persist Security Info=False;Integrated Security=false;PWD=P@ssw0rd;UID=SA;Initial Catalog=DapperDemo;server=localhost";

        public CustomerController(ILogger<CustomerController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            List<Customer> customers = new();
            using (IDbConnection db = new SqlConnection(_connectionString))
            {

                customers = db.Query<Customer>("Select * From Customers").ToList();
            }
            return View(customers);
        }

        public IActionResult Details(int id)
        {
            Customer customer = new();
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                var sql = $"Select * From (Select * from Customers LEFT OUTER JOIN Orders ON Customers.CustomerID = Orders.CID) AS T1 LEFT OUTER JOIN OrderDetails ON T1.OrderID = OrderDetails.OrderID WHERE CustomerID = ${id}";
                customer = db.Query<Customer, Order, OrderDetails, Customer>(sql,
                (customer, order, details) => { order.OrderDetails = details; customer.Order = order; return customer; },
                 new { id },
                 splitOn: "OrderID,OrderID").SingleOrDefault();
            }
            return View(customer);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Customer customer)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    string sqlQuery = "Insert Into Customers (FirstName, LastName, Email) Values(@FirstName, @LastName, @Email)";

                    int rowsAffected = db.Execute(sqlQuery, customer);
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Customer/Edit/5
        public IActionResult Edit(int id)
        {
            Customer customer = new();
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                customer = db.Query<Customer>("Select * From Customers WHERE CustomerID =" + id, new { id }).SingleOrDefault();
            }
            return View(customer);
        }

        // POST: Customer/Edit/5
        [HttpPost]
        public IActionResult Edit(Customer customer)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    string sqlQuery = "UPDATE Customers set FirstName='" + customer.FirstName +
                        "',LastName='" + customer.LastName +
                        "',Email='" + customer.Email +
                        "' WHERE CustomerID=" + customer.CustomerID;

                    int rowsAffected = db.Execute(sqlQuery);
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Customer/Delete/5
        [HttpGet]
        [Route("Customer/Delete/{id}")]
        public IActionResult GetDeletePage(int id)
        {
            Customer customer = new Customer();
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                customer = db.Query<Customer>("Select * From Customers WHERE CustomerID =" + id, new { id }).SingleOrDefault();
            }
            return View("Delete", customer);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    string sqlQuery = "Delete From Customers WHERE CustomerID = " + id;

                    int rowsAffected = db.Execute(sqlQuery);
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
