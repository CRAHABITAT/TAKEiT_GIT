using TickitNewFace.Models;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace TickitNewFace.Const
{
    /// <summary>
    /// Cette classe répertorie toutes les constantes de l'application ainsi que sa configuration générale.
    /// </summary>
    public class ApplicationConsts
    {

//        public const string HiqPdfSerialNumber = "YCgJMTAE-BiwJAhIB-EhlWTlBA-UEBRQFBA-U1FOUVJO-WVlZWQ==";
        public const string HiqPdfSerialNumber = "Fl5/R0Zy-cFp/dGR3-ZG8nJDgm-Nic2JzYv-LiMkNiUn-OCckOC8v-Ly8=";

        public const string dossierTraitementPdf = "E:\\inetpub\\product\\Content\\dossier_pdf\\";

        public static Dictionary<string, SqlConnection> connections = new Dictionary<string,SqlConnection>();

        public static string SessionID = System.Web.HttpContext.Current.Session.SessionID;
        
        // Configuration globale de l'application
        public static T_Configuration config;

        // Formats d'impression
        public const string format_Reglette = "Reglette";
        public const string format_A5_simple = "A5_simple";
        public const string format_A5_recto_verso = "A5_recto_verso";
        public const string format_A6_simple = "A6_simple";
        public const string format_A6_recto_verso = "A6_recto_verso";
        public const string format_A7_simple = "A7_simple";
        public const string format_A7_recto_verso = "A7_recto_verso";

        // Lieux de fabrication
        public const string made_in_FR = "FR";
        public const string made_in_IT = "IT";
        public const string made_in_ES = "ES";
        public const string made_in_DE = "DE";
        public const string made_in_PT = "PT";
        public const string made_in_BE = "BE";
        public const string made_in_EU = "EU";
        public const string made_in_UE = "UE";
//ajouter par Cillia
        public const string made_in_IN = "IN";
        public const string made_in_JP = "JP";
        public const string made_in_TH = "TH";
        public const string made_in_GB = "GB";



        
        // Code pays
        public const int codePays_GB = 2;
        public const int codePays_FR = 4;
        public const int codePays_DE = 5;
        public const int codePays_ES = 6;

        // Dimensions
        public const string symboleDimensionProfondeur = "P";
        public const string symboleDimensionDiametre = "D";
        public const string symboleDimensionLargeur = "l";
        public const string symboleDimensionHauteur = "H";
        public const string symboleDimensionLongueur = "L";
        public const string dimensionUniteMesure = "cm";
        public const string symboleSeparateurDimension = "x";

        // Ajout format impression (Oui / Non)
        public const string ajoutFormatImpression = "YES";
        public const string suppressionFormatImpression = "NO";

        // Code types des prix
        public const string typePrix_demarqueLocale = "O";
        public const string typePrix_solde = "S";
        public const string typePrix_promo = "P";
        public const string typePrix_permanent = "N";

        // Séparateur des décimaux
        public const string separateurDecimal = ".";

        // Séparateur des décimaux
        public const char separateurDate = '/';

        // Groupe AD non franchisé
        public const string nonFranchise = "users";

        // Serviront à l'historisation des évenements
        public const string sessionStart = "SESSION_START";
        public const string sessionEnd = "SESSION_END";
        public const string insertPrices = "INSERT_PRICES";
        public const string updateData = "INSERT_OR_UPDATE_DATA";

        public const string typePastillePromoReglette = "promo_rose";
        public const string typePastilleSoldeReglette = "promo_rouge";
        
    }
}
