using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QuickMailGenerator
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public Settings settings;
        private int fontSizeH1 = 30;
        private int fontSizeInputName = 24;
        private int fontSizeInputDescription = 14;
        private int fontSizeInput = 20;
        private int fontSizeMenu = 20;

        private Template openingTemplate;

        public MainWindow()
        {
            InitializeComponent();
            
            try
            {
                var settingsText = File.ReadAllText("Settings.json");

                
                settings = JsonConvert.DeserializeObject<Settings>(settingsText, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Populate });
                foreach (var menuItem in settings.MenuItems)
                {
                    Console.WriteLine("Id: {0}, Name: {1}", menuItem.Id, menuItem.Name);
                   
                    var button = new Button
                    {
                        Content = menuItem.Name,
                        Name = menuItem.Id,
                        FontSize = fontSizeMenu
                    };
                    button.Click += MenuButton_Click;
                    MenuPanel.Children.Add(button);              
                }

                if(settings.MenuItems.Count > 0)
                {
                    OpenTemplate(settings.MenuItems[0].Id);
                }           
                

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void OpenTemplate(string id)
        {
            TemplatePanel.Children.Clear();
            var template = settings.Templates.Find(t => t.Id == id);
            openingTemplate = template;
            if(template == null)
            {
                TemplatePanel.Children.Add(new TextBlock
                {
                    Text = $"Cannot find template: {id}",
                    FontSize = fontSizeH1
                });
            }else
            {
                TemplatePanel.Children.Add(new TextBlock
                {
                    Text = template.Name,
                    FontSize = fontSizeH1
                });

                if (template.Inputs != null)
                {
                    foreach (var input in template.Inputs)
                    {
                        Grid InputGrid = new Grid();
                        var margin = InputGrid.Margin;
                        margin.Top = 20;
                        InputGrid.Margin = margin;
                        
                        ColumnDefinition gridCol1 = new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) };
                        ColumnDefinition gridCol2 = new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) };
                        InputGrid.ColumnDefinitions.Add(gridCol1);
                        InputGrid.ColumnDefinitions.Add(gridCol2);

                        InputGrid.Children.Add(
                            new StackPanel
                            {
                                Children =
                                {
                                     new TextBlock
                                    {
                                        Name = "name",
                                        Text = input.Name,
                                        FontSize = fontSizeInputName
                                    },
                                      new TextBlock
                                    {
                                        Text = input.Description,
                                        FontSize = fontSizeInputDescription
                                    },
                                }
                            });


                        var inputStack = new StackPanel();
                        
                        var iMargin = inputStack.Margin;
                        iMargin.Top = 10;
                        inputStack.Margin = iMargin;
                        

                        TextBox inputBox = new TextBox();
                        switch (input.Type)
                        {
                            case InputType.Singleline:
                                inputBox = new TextBox
                                {
                                    Name = "input",
                                    Text = input.Default,
                                    FontSize = fontSizeInput,
                                    Height = double.NaN
                                    };
                                break;
                            case InputType.Multiline:
                                inputBox = new TextBox
                                {
                                    Text = input.Default,
                                    FontSize = fontSizeInput,
                                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                                    AcceptsReturn = true
                                };
                                break;
                        }
                        inputStack.Children.Add(inputBox);
                        inputStack.SetValue(Grid.ColumnProperty, 1);
                        InputGrid.Children.Add(inputStack);
                        TemplatePanel.Children.Add(InputGrid);
                    }
                }
                var mailButton = new Button
                {
                    Content = "メール作成",
                    FontSize = fontSizeH1
                };
                mailButton.Click += MailButton_Click;
                TemplatePanel.Children.Add(mailButton);
            }
        }

        private void MailButton_Click(object sender, RoutedEventArgs e)
        {
            CreateMailItem();

        }

        private List<KeyValuePair<string, string>> getInputs()
        {
            var inputs = new List<KeyValuePair<string, string>>();

            foreach (var child in TemplatePanel.Children)
            {
                Grid grid = child as Grid;
                if(grid != null)
                {
                    StackPanel stack = grid.Children[0] as StackPanel;
                    var nameBlock = stack.Children[0] as TextBlock;
                    Debug.WriteLine(nameBlock.Text);

                    StackPanel inputStack = grid.Children[1] as StackPanel;
                    var inputBox = inputStack.Children[0] as TextBox;
                    Debug.WriteLine(inputBox.Text);

                    inputs.Add(new KeyValuePair<string, string>(nameBlock.Text, inputBox.Text));                    
                }
            }

            return inputs;
        }

        private void CreateMailItem()
        {
            var inputs = getInputs();

            var mailDic = new Dictionary<string, string>();
            mailDic.Add("to", openingTemplate.MailTo);
            mailDic.Add("cc", openingTemplate.MailCc);
            mailDic.Add("bcc", openingTemplate.MailBcc);
            mailDic.Add("title", openingTemplate.MailTitle);
            mailDic.Add("content", openingTemplate.MailContent);
            
            for (int i = inputs.Count - 1; i >= 0; i--)
            {                
                foreach (string key in mailDic.Keys.ToList())
                {
                    mailDic[key] = mailDic[key].Replace("{" + inputs[i].Key + "}", inputs[i].Value);
                }               
            }

            foreach (string key in mailDic.Keys.ToList())
            {
                mailDic[key] = mailDic[key].Replace("\n", "%0A");
            }
            
            var url = $"mailto:{mailDic["to"]}?cc={mailDic["cc"]}&bcc={mailDic["bcc"]}";
            url += $"&subject={mailDic["title"]}&body={mailDic["content"]}";
            Debug.WriteLine(url);
            
            Process.Start(url);
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            OpenTemplate(button.Name);
            Console.WriteLine(button.Name);
        }
    }
}
