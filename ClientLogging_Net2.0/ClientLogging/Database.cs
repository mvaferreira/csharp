﻿using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Reflection;

namespace ClientLogging
{
    class Database
    {
        string localPath = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        string dbFile;
        string strConn;
        SQLiteConnection conn;
        SQLiteCommand cmd;
        SQLiteDataReader dr;

        public Database()
        {
            dbFile = localPath + "\\ClientLogging.db";
            strConn = "Data Source=" + dbFile;

            CreateDB();
        }
        private void CreateDB()
        {
            string createDBSQL = @"CREATE TABLE Clients(
                                      ID INTEGER PRIMARY KEY AUTOINCREMENT,
                                      ClientName TEXT,
                                      ClientIP TEXT NOT NULL,
                                      ClientPort TEXT NOT NULL,
                                      DestinationIP TEXT NOT NULL,
                                      DestinationPort TEXT NOT NULL,
                                      Protocol TEXT NOT NULL,
                                      Count INTEGER NOT NULL,
                                      Timestamp TEXT NOT NULL
                                   );";

            if (!File.Exists(dbFile))
            {
                SQLiteConnection.CreateFile(dbFile);

                using (conn = new SQLiteConnection(strConn))
                {
                    conn.Open();

                    using (cmd = new SQLiteCommand(createDBSQL, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private int CheckClient(string clientIP, string destinationPort, string protocol)
        {
            int clientCount = 0;
            string checkClientSQL = "SELECT ClientIP, Count, DestinationPort, Protocol " + 
                                    "FROM Clients " + 
                                    "WHERE ClientIP='" + clientIP + "' " + 
                                    "AND DestinationPort='" + destinationPort + "' " + 
                                    "AND Protocol='" + protocol + "'";

            using (conn = new SQLiteConnection(strConn))
            {
                conn.Open();

                using (cmd = new SQLiteCommand(checkClientSQL, conn))
                {
                    dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        clientCount = Convert.ToInt32(dr["Count"]);
                    }

                    if (!dr.IsClosed)
                    {
                        dr.Close();
                    }
                }
            }

            return clientCount;
        }

        public void AddClient(string clientName, string clientIP, string clientPort, string destinationIP, string destinationPort, string protocol, string timestamp)
        {
            string clientSQL;
            int howMany = CheckClient(clientIP, destinationPort, protocol);
            
            if (howMany == 0)
            {
                howMany++;
                Console.WriteLine("[{0}] {1} [{2}] -> {3}/{4}", timestamp, clientName, clientIP, protocol, destinationPort);
                clientSQL = "INSERT into Clients(ClientName,ClientIP,ClientPort,DestinationIP,DestinationPort,Protocol,Count,Timestamp) " +
                              "VALUES('" + clientName + "','" + clientIP + "','" + clientPort + "','" + destinationIP + "','" +
                              destinationPort + "','" + protocol + "'," + howMany + ",'" + timestamp + "')";
            }
            else
            {
                howMany++;
                clientSQL = "UPDATE Clients SET Count ='" + howMany + "', Timestamp ='" + timestamp + "' " +
                                "WHERE ClientIP = '" + clientIP + "'" +
                                "AND DestinationPort ='" + destinationPort + "'" +
                                "AND Protocol ='" + protocol + "'";
            }

            using (conn = new SQLiteConnection(strConn))
            {
                conn.Open();

                using (cmd = new SQLiteCommand(clientSQL, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void ShowClients()
        {
            string checkClientSQL = "SELECT ClientName, ClientIP, Protocol, DestinationPort, Count " +
                                    "FROM Clients Order by ClientIP, Protocol, DestinationPort";

            using (conn = new SQLiteConnection(strConn))
            {
                conn.Open();

                using (cmd = new SQLiteCommand(checkClientSQL, conn))
                {
                    dr = cmd.ExecuteReader();

                    string client = string.Empty;

                    while (dr.Read())
                    {
                        string dbClient = dr["ClientIP"].ToString();

                        if (dbClient != client)
                        {
                            Console.WriteLine("{0} [{1}]", dr["ClientName"], dr["ClientIP"]);
                            client = dbClient;
                        }

                        Console.WriteLine("\t{0}/{1} - {2}", dr["Protocol"], dr["DestinationPort"], dr["Count"]);
                    }

                    if (!dr.IsClosed)
                    {
                        dr.Close();
                    }
                }
            }
        }

        public List<string> ExportClients()
        {
            string checkClientSQL = "SELECT * " +
                                    "FROM Clients Order by ClientIP, Protocol, DestinationPort";

            List<string> clients = new List<string>();

            using (conn = new SQLiteConnection(strConn))
            {
                conn.Open();

                using (cmd = new SQLiteCommand(checkClientSQL, conn))
                {
                    dr = cmd.ExecuteReader();

                    string client = string.Empty;

                    clients.Add("Timestamp,ClientName,ClientIP,ClientPort,DestinationIP,DestinationPort,Protocol,Count" + Environment.NewLine);

                    while (dr.Read())
                    {
                        clients.Add(dr["Timestamp"] + "," +
                                    dr["ClientName"] + "," +
                                    dr["ClientIP"] + "," +
                                    dr["ClientPort"] + "," +
                                    dr["DestinationIP"] + "," +
                                    dr["DestinationPort"] + "," +
                                    dr["Protocol"] + "," +
                                    dr["Count"] + Environment.NewLine);
                    }

                    if (!dr.IsClosed)
                    {
                        dr.Close();
                    }
                }
            }

            return clients;
        }
    }
}