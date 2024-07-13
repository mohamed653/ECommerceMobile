using Microsoft.Extensions.Configuration;
using System.IO;

namespace ECommerceApi.StaticLinks
{
    public enum NotificationType
    {
        CustomerOrder,
        CustomerProductList,
        CustomerOffers,
        AdminOrder,
        AdminProductCRUD,
        AdminOffers,
        AdminStock,
        User,
        Admin
    }

    public static class NotificationLinks
    {
        private static readonly string _baseUrl;

        static NotificationLinks()
        {
            // Load configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            _baseUrl = configuration.GetSection("AppSettings")["FrontBaseUrl"];
        }

        public static string GetLink(NotificationType type, int id = 0)
        {
            switch (type)
            {
                case NotificationType.CustomerOrder:
                    return $"{_baseUrl}customer-products/customer-previous-orders/{id}";
                case NotificationType.CustomerProductList:
                    return $"{_baseUrl}home";
                case NotificationType.CustomerOffers:
                    return $"{_baseUrl}customer-products/customer-offers";
                case NotificationType.AdminOrder:
                    return $"{_baseUrl}admin-products/admin-previous-orders/{id}";
                case NotificationType.AdminProductCRUD:
                    return $"{_baseUrl}admin-products/admin-product-crud";
                case NotificationType.AdminOffers:
                    return $"{_baseUrl}admin-products/admin-offers";
                case NotificationType.AdminStock:
                    return $"{_baseUrl}admin-products/admin-stock";
                case NotificationType.User:
                    return $"{_baseUrl}user";
                case NotificationType.Admin:
                    return $"{_baseUrl}admin";
                default:
                    return _baseUrl;
            }
        }
    }
}
