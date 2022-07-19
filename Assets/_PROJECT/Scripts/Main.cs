using System.Collections.Generic;
using System.IO;
using ProductCatalog;
using ProductCatalog.Data;
using UnityEngine;
using UnityEngine.UIElements;

namespace _PROJECT.Scripts
{
    public class Main : MonoBehaviour
    {
        [SerializeField] private UIDocument _uiDocument;
        [SerializeField] private VisualTreeAsset _itemPrefab;
    
        private List<IProductCatalogEntry> _tempItems;

        private void Start()
        {
            var productCatalog = GetProductCatalog();
            SortByPrice();

            _uiDocument.rootVisualElement.Q<Button>("SortByPrice").clicked += SortByPrice;
        
            _uiDocument.rootVisualElement.Q<Button>("SortByName").clicked += () => 
                PopulateScroll(productCatalog.SortByName(false));
        
            _uiDocument.rootVisualElement.Q<Button>("SortByItems").clicked += () =>
                PopulateScroll(productCatalog.SortByItems(new[] {ItemTypes.Coins, ItemTypes.Gems, ItemTypes.Tickets}));
        
            _uiDocument.rootVisualElement.Q<Button>("SortByItems2").clicked += () => 
                PopulateScroll(productCatalog.SortBy(entry => entry.Item, new ProductCatalogComparers.ItemComparer(new[] {ItemTypes.Tickets, ItemTypes.Coins, ItemTypes.Gems})));
        
            _uiDocument.rootVisualElement.Q<Button>("FilterByPrice").clicked += () =>
                PopulateScroll(productCatalog.FilterBy<uint>(100, entry => entry.Price, Comparer<uint>.Default));
        
            _uiDocument.rootVisualElement.Q<Button>("FilterByCoinsAndTickets").clicked += () => 
                PopulateScroll(productCatalog.FilterByCoinsAndTickets());
        
            _uiDocument.rootVisualElement.Q<Button>("FilterByCoinsOrTickets").clicked += () => 
                PopulateScroll(productCatalog.FilterByCoinsOrTickets());
        
            _uiDocument.rootVisualElement.Q<Button>("FilterByPrice50_150").clicked += () =>
                PopulateScroll(productCatalog.FilterBy<uint>(50, 150, entry => entry.Price, Comparer<uint>.Default));

            void SortByPrice()
            {
                PopulateScroll(productCatalog.SortByPrice());
            }
        }

        private void PopulateScroll(IEnumerable<IProductCatalogEntry> entries)
        {
            _tempItems = new List<IProductCatalogEntry>(entries);
            var listView = _uiDocument.rootVisualElement.Q<ListView>("Scroll");
            listView.itemsSource = _tempItems;
            listView.makeItem = () => _itemPrefab.Instantiate();
        
            listView.bindItem =  (element, i) =>
            {
                element.Q<Label>("Name").text = _tempItems[i].Name;
                element.Q<Label>("Price").text = $"{_tempItems[i].Price / 10} $";
                element.Q<Label>("Description").text = _tempItems[i].Description;
                for (var j = 0; j < 3; j++)
                {
                    element.Q<VisualElement>($"Item{j}").visible = false;
                }
                var item = _tempItems[i].Item;
                var amount = _tempItems[i].Amount;
                for (var j = 0; j < item.Length; j++)
                {
                    var itemElement = element.Q<VisualElement>($"Item{j}");
                    itemElement.visible = true;
                    itemElement.Q<Label>("Item").text = item[j].ToString();
                    itemElement.Q<Label>("Amount").text = amount[j].ToString();
                }
            };
        }

        private static ProductCatalog.ProductCatalog GetProductCatalog()
        {
            var path = $"{Application.streamingAssetsPath}/data.json";
            var json = File.ReadAllText(path);

            var loader = new ProductCatalogLoader(json);
            return loader.Build();
        }
    }
}
