﻿using Newtonsoft.Json;
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
        private int fontSizeMenu = 18;

        private Template openingTemplate;

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                var settingsText = File.ReadAllText("config/Settings.json");

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

                if (settings.GeneralSettings.Inputs == null)
                {
                    settings.GeneralSettings.Inputs = new List<Input>();
                }

                foreach (var template in settings.Templates)
                {
                    if (template.MailBody == null)
                    {
                        template.MailBody = new List<string>();
                    }

                    if (template.Inputs == null)
                    {
                        template.Inputs = new List<Input>();
                    }
                    foreach (var input in template.Inputs)
                    {
                        var sameIdInput = settings.GeneralSettings.Inputs.FirstOrDefault(x => x.Id == input.Id);
                        if (sameIdInput != null)
                        {
                            if (input.Name == null)
                            {
                                input.Name = sameIdInput.Name;
                            }
                            if (input.Description == null)
                            {
                                input.Description = sameIdInput.Description;
                            }
                            if (input.Default == null)
                            {
                                input.Default = sameIdInput.Default;
                            }
                            if (input.Type == InputType.Null)
                            {
                                input.Type = sameIdInput.Type;
                            }
                        }

                        if (input.Default == null)
                        {
                            input.Default = "";
                        }

                        if (input.Description == null)
                        {
                            input.Description = "";
                        }

                        if (input.Type == InputType.Null)
                        {
                            input.Type = InputType.Singleline;
                        }
                    }
                }

                if (settings.MenuItems.Count > 0)
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
            if (template == null)
            {
                TemplatePanel.Children.Add(new TextBlock
                {
                    Text = $"Cannot find template: {id}",
                    FontSize = fontSizeH1
                });
            }
            else
            {
                TemplatePanel.Children.Add(new TextBlock
                {
                    Text = template.Name,
                    FontSize = fontSizeH1
                });

                bool firstElement = true;

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
                                        Name = input.Id,
                                        Text = input.Name,
                                        FontSize = fontSizeInputName,
                                        TextWrapping=TextWrapping.Wrap
                                    },
                                      new TextBlock
                                    {
                                        Text = input.Description,
                                        FontSize = fontSizeInputDescription,
                                        TextWrapping=TextWrapping.Wrap
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
                                AcceptsReturn = true,
                                MinLines = 3
                            };
                            break;
                    }
                    inputBox.TextChanged += InputBox_TextChanged;

                    inputStack.Children.Add(inputBox);
                    inputStack.SetValue(Grid.ColumnProperty, 1);
                    InputGrid.Children.Add(inputStack);
                    TemplatePanel.Children.Add(InputGrid);

                    if (firstElement)
                    {
                        inputBox.Focus();
                        firstElement = false;
                    }
                }

                var mailButton = new Button
                {
                    Content = "メール作成",
                    FontSize = fontSizeH1
                };

                var buttonMargin = mailButton.Margin;
                buttonMargin.Top = 10;
                mailButton.Margin = buttonMargin;

                mailButton.Click += MailButton_Click;
                TemplatePanel.Children.Add(mailButton);
            }
            UpdatePreview();
        }

        private void InputBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdatePreview();
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
                if (grid != null)
                {
                    StackPanel stack = grid.Children[0] as StackPanel;
                    var nameBlock = stack.Children[0] as TextBlock;
                    Debug.WriteLine(nameBlock.Text);

                    StackPanel inputStack = grid.Children[1] as StackPanel;
                    var inputBox = inputStack.Children[0] as TextBox;
                    Debug.WriteLine(inputBox.Text);

                    inputs.Add(new KeyValuePair<string, string>(nameBlock.Name, inputBox.Text));
                }
            }

            return inputs;
        }

        private Dictionary<string, string> GetMailElements()
        {
            if (openingTemplate == null)
            {
                return null;
            }
            var inputs = getInputs();

            var mailDic = new Dictionary<string, string>();
            mailDic.Add("to", openingTemplate.MailTo);
            mailDic.Add("cc", openingTemplate.MailCc);
            mailDic.Add("bcc", openingTemplate.MailBcc);
            mailDic.Add("subject", openingTemplate.MailSubject);
            mailDic.Add("body", string.Join("\n", openingTemplate.MailBody));

            for (int i = inputs.Count - 1; i >= 0; i--)
            {
                foreach (string key in mailDic.Keys.ToList())
                {
                    mailDic[key] = mailDic[key].Replace("{" + inputs[i].Key + "}", inputs[i].Value);
                }
            }
            /*
            foreach (string key in mailDic.Keys.ToList())
            {
                mailDic[key] = mailDic[key].Replace("\n", "%0A");
            }
            */
            return mailDic;
        }

        private void UpdatePreview()
        {
            var mailDic = GetMailElements();

            string to = "", cc = "", bcc = "", subject = "", body = "";
            if (mailDic != null)
            {
                to = mailDic["to"];
                cc = mailDic["cc"];
                bcc = mailDic["bcc"];
                subject = mailDic["subject"];
                body = mailDic["body"];

            }
            ToPreview.Text = to;
            CcPreview.Text = cc;
            BccPreview.Text = bcc;
            SubjectPreview.Text = subject;
            BodyPreview.Text = body;

        }

        private void CreateMailItem()
        {
            /*
            var inputs = getInputs();

            var mailDic = new Dictionary<string, string>();
            mailDic.Add("to", openingTemplate.MailTo);
            mailDic.Add("cc", openingTemplate.MailCc);
            mailDic.Add("bcc", openingTemplate.MailBcc);
            mailDic.Add("subject", openingTemplate.MailSubject);
            mailDic.Add("body", string.Join("\n", openingTemplate.MailBody));

            for (int i = inputs.Count - 1; i >= 0; i--)
            {
                foreach (string key in mailDic.Keys.ToList())
                {
                    mailDic[key] = mailDic[key].Replace("{" + inputs[i].Key + "}", inputs[i].Value);
                }
            }
            */

            var mailDic = GetMailElements();

            foreach (string key in mailDic.Keys.ToList())
            {
                mailDic[key] = mailDic[key].Replace("\n", "%0A");
            }

            var url = $"mailto:{mailDic["to"]}?";
            if (mailDic["cc"] != "")
            {
                url += $"cc={ mailDic["cc"]}&";
            }
            if (mailDic["bcc"] != "")
            {
                url += $"bcc={ mailDic["bcc"]}&";
            }
            if (mailDic["subject"] != "")
            {
                url += $"subject={mailDic["subject"]}&";
            }
            if (mailDic["body"] != "")
            {
                url += $"body={mailDic["body"]}";
            }
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
