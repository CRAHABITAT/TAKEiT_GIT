using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Threading;
using System.Globalization;
using System.Data.SqlClient;
using System.Configuration;
using TickitNewFace.DAO;
using TickitNewFace.Models;

namespace TickitNewFace
{
    // Version DRISS 14/12/2017.
    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );
        }
        
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }
        
        // A début de la session
        protected void Session_Start()
        {
            string userName = User.Identity.Name.Substring(8).ToLower();
            
            // userName = "manager.finland";
            // userName = "directeur.madagascar";
            // userName = "manager.finland";
            // userName = "Directeur.Domus";
            // userName = "Manager.singapour";
            // userName = "directeur.guyane";
            // userName = "directeur.catalan";
            // userName = "directeur.takeit";
            // userName = "directeur.guatemala";
            // userName = "directeur.st-martin";
            // userName = "manager.thailand";
            // userName = "manager.philippines";
            // userName = "Manager.qatar";
            // userName = "magasin.reunion";
            // userName = "mcarbonnel";
            // userName = "msaintetienne";
            // userName = "BcnVisual";
            // userName = "directeur.catalan";
            // userName = "directeur.portet";
            // userName = "directeur.belgique";
            // userName = "directeur.ajaccio";
            // userName = "dsaintetienne";
            // userName = "dir.martinique";
            // userName = "store.milano";
            // userName = "dmeyrin";
            // userName = "msridi";
            // userName = "scrennes";
            // userName = "nmeradji";
            // userName = "msridi";
            // userName = "oouba";

            Session["userName"] = userName;

            if (null == Session["magid"])
                Session["magid"] = DAO.ActiveDirectory.getMagasinByLogin(userName);
            if (null == Session["franchiseName"])
                Session["franchiseName"] = DAO.ActiveDirectory.getFranchiseName(userName);
            if (null == Session["isAdmin"])
                Session["isAdmin"] = DAO.ActiveDirectory.isAdmin(userName);
            
            // Création de la connexion à la base de données.
            string connectionString = ConfigurationManager.ConnectionStrings["TickitConnectionString"].ConnectionString;
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            Const.ApplicationConsts.connections.Add(Session.SessionID, connection);
            
            // Initialisation de la configuration globale au démarage de l'application.
            Const.ApplicationConsts.config = DAO.ConfigurationDao.getConfigurationAppication();

            // Hsitorisation de l'évenement connexion.
            T_Evenement objEve = new T_Evenement();
            objEve.Dateve = DateTime.Now;
            objEve.Eve = Const.ApplicationConsts.sessionStart;
            objEve.Login = userName;
            EvenementDao.insertEvenement(objEve);
        }
        
        // A la fin de la session
        protected void Session_End()
        {
            SqlConnection connection;
            Const.ApplicationConsts.connections.TryGetValue(Session.SessionID, out connection);
            connection.Dispose();
            connection.Close();
            Const.ApplicationConsts.connections.Remove(Session.SessionID);
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            if (Request.UserLanguages != null)
            {
                string Lang = Request.UserLanguages[0];
                Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(Lang);
            }
            else 
            {
                Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("fr");
            }
        }
    }
}
