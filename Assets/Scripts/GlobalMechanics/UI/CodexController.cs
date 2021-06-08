using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;

namespace GlobalMechanics.UI
{
    public class CodexController : MonoBehaviour, ISmartphoneService
    {
        
        public TextAsset codex;
        public GameObject articlePanel;
        public TextMeshProUGUI titleHolder;
        public GameObject buttonPrefab;
        public RectTransform prefabHolder;
        private enum DisplayMode
        {
            Categories,
            ArticlesInCategory,
            Article
        }

        private DisplayMode _currentMode = DisplayMode.Categories;
        private Article _currentArticle;
        private Category _currentCategory;
        
        private Dictionary<string, Category> _categories;
        private List<GameObject> _buttonList;
        private TextMeshProUGUI _articleTextHolder;
        
        
        public bool IsOpened()
        {
            return this.gameObject.activeInHierarchy;
        }

        public void Open()
        {
            this.gameObject.SetActive(true);
            if (_categories == null)
            {
                _categories = new Dictionary<string, Category>();
                ExtractCodex();
                _articleTextHolder = articlePanel.GetComponentInChildren<TextMeshProUGUI>();
                _buttonList = new List<GameObject>();
            }
            ShowCategoryCatalogue();
            
        }
        
        public void Home()
        {
            this.gameObject.SetActive(false);
        }
        
        public void Back()
        {
            switch (_currentMode)
            {
                case DisplayMode.Article:
                    CloseArticle();
                    ShowArticleCatalogue(_currentCategory.Name);
                    break;
                case DisplayMode.Categories:
                    this.gameObject.SetActive(false);
                    break;
                case DisplayMode.ArticlesInCategory:
                    ShowCategoryCatalogue();
                    break;
            }
        }

        public void OpenEntry(string keyEntry)
        {
            switch (_currentMode)
            {
                case DisplayMode.Categories:
                    ShowArticleCatalogue(keyEntry);
                    break;
                case DisplayMode.ArticlesInCategory:
                    ShowArticle(keyEntry);
                    break;
                default:
                    return;
            }
        }

        private void CloseArticle()
        {
            _currentArticle = null;
            articlePanel.SetActive(false);
            titleHolder.text = _currentCategory.Name;
        }

        private void ShowArticle(string article)
        {
            Debug.Assert(_currentCategory.GetArticleList().Contains(article));

            _currentArticle = _currentCategory.GetArticle(article);
            articlePanel.SetActive(true);
            _articleTextHolder.text = _currentArticle.Text;
            titleHolder.text = _currentArticle.Name;
            _currentMode = DisplayMode.Article;
        }

        private void ShowArticleCatalogue(string category)
        {
            Debug.Assert(_categories.ContainsKey(category));
            foreach (var button in _buttonList)
            {
                Destroy(button);
            }
            _buttonList.Clear();
            
            _currentCategory = _categories[category];
            titleHolder.text = category;
            foreach (var article in _currentCategory.GetArticleList())
            {
                GameObject button = Instantiate(buttonPrefab, prefabHolder);
                button.GetComponent<CodexEntry>().SetKey(_currentCategory.GetArticle(article).Name, this);
                _buttonList.Add(button);
            }
            _currentMode = DisplayMode.ArticlesInCategory;
        }

        private void ShowCategoryCatalogue()
        {
            titleHolder.text = "База Знаний";
            foreach (var button in _buttonList)
            {
                Destroy(button);
            }
            _buttonList.Clear();
            foreach (var category in _categories.Values)
            {
                GameObject button = Instantiate(buttonPrefab, prefabHolder);
                var t = button.GetComponent<CodexEntry>();
                t.SetKey(category.Name, this, category.GetArticleList().Length);
                _buttonList.Add(button);
            }
            _currentMode = DisplayMode.Categories;
        }

        private void ExtractCodex()
        {
            JObject codexJObject = JObject.Parse(codex.text);
            JToken categoriesToken = codexJObject["catalogue"];
            foreach (var categoryToken in categoriesToken)
            {
                var categoryName = categoryToken["name"].Value<string>();
                var articleTokens = categoryToken["articles"];
                List<Article> articlesInCategory = new List<Article>();
                foreach (var article in articleTokens)
                {
                    articlesInCategory.Add(new Article(article["name"].Value<string>(), article["text"].Value<string>(), categoryName));
                }
                _categories.Add(categoryName, new Category(categoryName, articlesInCategory));
            }
        }
        
        private class Category
        {
            private readonly string _name;
            private readonly Dictionary<string, Article> _articles;

            public string Name => _name;

            public Article GetArticle(string key)
            {
                Debug.Assert(_articles.ContainsKey(key));
                return _articles[key];
            }

            public string[] GetArticleList()
            {
                var result = new string[_articles.Count];
                _articles.Keys.CopyTo(result, 0);
                return result;
            }

            public Category(string categoryName, ICollection<Article> articlesInCategory)
            {
                _name = categoryName;
                _articles = new Dictionary<string, Article>();
                foreach (var article in articlesInCategory)
                {
                    _articles.Add(article.Name, article);   
                }
            }
        }

        private class Article
        {
            private readonly string _name;
            private readonly string _text;
            private readonly string _category;

            public string Name => _name;
            public string Text => _text;
            public string HomeCategory => _category;

            public Article(string name, string text, string category)
            {
                _name = name;
                _text = text;
                _category = category;
            }
        }
       
    }
}