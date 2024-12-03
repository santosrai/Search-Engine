using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SearchEngine
{


    public partial class SearchForm : Form
    {
        // Declare class-level field

        private Panel resultsPanel;
        private Label welcomeLabel;

        private Panel topBarPanel;
        private Panel sidebarPanel;
        private Button toggleButton; // Button to toggle sidebar
        private Timer sidebarTimer; // Timer for animation
        private bool isSidebarVisible = true; // Track sidebar visibility

        // Declare a FlowLayoutPanel for chat messages
        private FlowLayoutPanel chatPanel;
        private bool isFirstSearch = true; // Track if it's the first search

        private Panel allResultsPanel;
        private List<string> _recentQueries = new List<string>();

        private Label loadingLabel;


        // store and manage query and result history
        private List<(string, IEnumerable<SearchData>)> _queryHistory = new List<(string, IEnumerable<SearchData>)>();

        // For SerpiApi
        private SerpApiService _serpApiService;
        public SearchForm()
        {
            InitializeComponent();
            InitializeSearchProgram();
            InitializeSidebarToggle(); // Initialize the sidebar toggle
            SetupUI();
        }

        private void InitializeSearchProgram()
        {

            /*  var sourceDirPath = Directory.GetCurrentDirectory() + @"\data\search-data-M.txt";
             _searchProgram = new Program(sourceDirPath);*/

            // Initialize SerpApi with API key
         
            string apiKey = "";
            _serpApiService = new SerpApiService(apiKey);

           

        }

        private void InitializeSidebarToggle()
        {

            // Path to the sidebar icon
            string sidbarImgPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "img", "sidebar.png");


            // Toggle Button
            PictureBox toggleIcon = new PictureBox
            {
                Width = 20,
                Height = 20,
                Location = new Point(10, 10), // Position to the right of the sidebar
                Image = Image.FromFile(sidbarImgPath), // Set the path to your icon image
                SizeMode = PictureBoxSizeMode.StretchImage, // Adjust size mode
                Cursor = Cursors.Hand // Change cursor to hand on hover
            };
            toggleIcon.Click += ToggleButton_Click; // Use the same event handler
            this.Controls.Add(toggleIcon); // Ensure it's added to the form
            toggleIcon.BringToFront();

            // Initialize the timer
            sidebarTimer = new Timer();
            sidebarTimer.Interval = 15; // Adjust for speed of animation
            sidebarTimer.Tick += SidebarTimer_Tick;

             
             
            // Path to the new chat icon
            string newChatImgPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "img", "new-chat.png");

           

            // Refresh Icon (using PictureBox)
            PictureBox newChatIcon = new PictureBox
            {
                Width = 20,
                Height = 20,
                Location = new Point(toggleIcon.Right + 10, 10), // Position next to the toggle button
                Image = Image.FromFile(newChatImgPath), // Load the image
                SizeMode = PictureBoxSizeMode.StretchImage, // Adjust size mode
                Cursor = Cursors.Hand // Change cursor to hand on hover
            };
            newChatIcon.Click += RefreshButton_Click; // Event handler for refresh
            this.Controls.Add(newChatIcon); // Add to the form
            newChatIcon.BringToFront();

            // Initialize the timer
            sidebarTimer = new Timer();
            sidebarTimer.Interval = 15; // Adjust for speed of animation
            sidebarTimer.Tick += SidebarTimer_Tick;
             }

             private void RefreshButton_Click(object sender, EventArgs e)
            {
                // Logic to refresh chat results
                RefreshChatResults();
            }

            private void RefreshChatResults()
            {
                 // Show the welcome message again
                welcomeLabel.Show();
                // clear the chat panel and reload data
                chatPanel.Controls.Clear(); 
                
            }

        private void ToggleButton_Click(object sender, EventArgs e)
        {
            // Start the animation
            sidebarTimer.Start();
        }

        private void SidebarTimer_Tick(object sender, EventArgs e)
        {
            if (isSidebarVisible)
            {
                // Animate sidebar collapsing
                if (sidebarPanel.Width > 0)
                {
                    sidebarPanel.Width -= 10; // Adjust for speed of collapse
                }
                else
                {
                    sidebarTimer.Stop();
                    isSidebarVisible = false;
                }
            }
            else
            {
                // Animate sidebar expanding
                if (sidebarPanel.Width < 200) // Original width
                {
                    sidebarPanel.Width += 10; // Adjust for speed of expand
                }
                else
                {
                    sidebarTimer.Stop();
                    isSidebarVisible = true;
                }
            }
             
        }

        private void SetupUI()
        {
            // Set form properties
            this.Text = "SearchForm";
            this.Width = 850;
            this.Height = 650;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;


            // Top Header
            topBarPanel = new Panel
            {
                Width = 800,
                Dock = DockStyle.Top,
                Height = 40,
                BackColor = Color.FromArgb(45, 45, 45)
            };

            this.Controls.Add(topBarPanel);
            


            // Sidebar Panel
            sidebarPanel = new Panel
            {
                Width = 200,
                Dock = DockStyle.Left,
                BackColor = Color.FromArgb(45, 45, 45)
            };
            this.Controls.Add(sidebarPanel);

            // Recent Queries Section
            Label recentLabel = new Label
            {
                Text = "Recent",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Top,
                Height = 30,
                Width = 100,
                    Padding = new Padding(10)
            };
            sidebarPanel.Controls.Add(recentLabel);

                // Panel to hold recent queries
                FlowLayoutPanel recentQueriesPanel = new FlowLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    AutoScroll = true,
                    BackColor = Color.FromArgb(45, 45, 45)
                };
            sidebarPanel.Controls.Add(recentQueriesPanel);

            // Main content panel
            FlowLayoutPanel mainPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown, // Controls flow vertically
                WrapContents = false,
                AutoScroll = true,
                Visible = true,
                BackColor = Color.FromArgb(40, 40, 40) // Slightly lighter than the sidebar
            };

            this.Controls.Add(mainPanel);

            

            // Add scrollable results panel
            resultsPanel = new Panel
            {
                
                Width = 600,
                Height = 450,
                AutoScroll = true,
                
            };
            mainPanel.Controls.Add(resultsPanel);
           
           // Welcome Label
             welcomeLabel = new Label
            {
                Text = "What can I help with?",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                AutoSize = true,
                ForeColor = Color.White
            };

  
            // Calculate center position
            int centerX = 200 ;
            int centerY = (this.ClientSize.Height - welcomeLabel.Height) / 2;

            welcomeLabel.Location = new Point(centerX, centerY);

        
            resultsPanel.Controls.Add(welcomeLabel);

               // Initialize a panel to hold all results
                allResultsPanel = new Panel
                {
                    Dock = DockStyle.Top,
                    AutoSize = true,
                    BackColor = Color.FromArgb(40, 40, 40), // Background color for results
                    Padding = new Padding(10)
                };
                resultsPanel.Controls.Add(allResultsPanel); // Add to chatPanel

         

            // Bottom input area
            Panel inputPanel = new Panel
            {
         
                Dock = DockStyle.Bottom, // Dock to the bottom
                
            };
            mainPanel.Controls.Add(inputPanel);


            // inputSearchBox
            TextBox inputSearchBox = new TextBox
            {
            
                Multiline = true,
                Font = new Font("Segoe UI", 12),
                Height = 100,
                
                MaximumSize = new Size(400, 100), // Set maximum height to 100
                ScrollBars = ScrollBars.None, // hidee scrollbars
                BorderStyle = BorderStyle.FixedSingle
            };

         
            inputSearchBox.Width = 400; // Set a fixed width

            // Calculate the vertical position to center it
            int inSearchBoxY = (inputPanel.Height - inputSearchBox.Height) / 2;

            // Calculate the horizontal position to center it
            int inSearchBoxX = (inputPanel.Width - inputSearchBox.Width) / 2;

            // Set the location of the inputSearchBox
            inputSearchBox.Location = new Point(50, inSearchBoxY); // Center both horizontally and vertically

            inputPanel.Controls.Add(inputSearchBox);

           


            // Send button
            Button searchButton = new Button
            {
                Text = "Search",
                Width = 80,
                Height = 35,
                Location = new Point(inputSearchBox.Right +10, 12),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 120, 215), // Primary color
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold), // Modern font
                Cursor = Cursors.Hand // Change cursor to hand on hover
            };
            searchButton.FlatAppearance.BorderSize = 0;
           

            searchButton.Click += (object sender, EventArgs e) =>
            {
                string query = inputSearchBox.Text.Trim();

                if (string.IsNullOrEmpty(query))
                {
                    MessageBox.Show("Please enter a search query", "Search Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                searchButton_Click(sender, e, query);
            };


            inputPanel.Controls.Add(searchButton);
           
          // Initialize chat panel
            chatPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                Width = 600,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                BackColor = Color.FromArgb(40, 40, 40) // Background color for chat
               
                
            };
            resultsPanel.Controls.Add(chatPanel); // Add chatPanel to mainPanel
            
            mainPanel.BringToFront();

        }

     private void AddChatMessage(string title, string url, string description, Image thumbnail, bool isUserQuery)
    {
        // Create a panel for the chat message
        Panel messagePanel = new Panel
        {
            Width = chatPanel.Width - 100,
            AutoSize = true,
            AutoScroll = true,
            BackColor = isUserQuery ? Color.FromArgb(0, 120, 215) : Color.FromArgb(50, 50, 50), // Different background color
            Margin = new Padding(10),
            Padding = new Padding(10),
            Dock = isUserQuery ? DockStyle.Right : DockStyle.Left // Position based on who sent the message
        };
        //  // Loading Indicator
        //     loadingLabel = new Label
        //     {
        //         Text = "Loading...",
        //         ForeColor = Color.White,
        //         BackColor = Color.FromArgb(40, 40, 40),
        //         AutoSize = true,
        //         Visible = false, // Initially hidden
        //         Dock = DockStyle.Top,
        //         TextAlign = ContentAlignment.MiddleCenter
        //     };
        //     this.Controls.Add(messagePanel);

        // Title Label (Top)
        Label titleLabel = new Label
        {
            Text = title,
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            ForeColor = Color.White, // Set title color to white
            AutoSize = true,
            Margin = new Padding(5),
            Location = new Point(10,10),
            
        };
        messagePanel.Controls.Add(titleLabel);

        // Declare linkLabel variable
        LinkLabel linkLabel = null;
        
         // LinkLabel for URL (Middle)
        if (!string.IsNullOrEmpty(url))
        {
            linkLabel = new LinkLabel
            {
                Text = url,
                AutoSize = true,
                ForeColor = Color.White, // Set link color to white
                Margin = new Padding(5),
                Dock = DockStyle.None,
                Location = new Point(10, titleLabel.Bottom + 2),
                MaximumSize = new Size(chatPanel.Width - 100, 0), // Optional: Set a maximum width for wrapping
            };
            linkLabel.LinkClicked += (sender, e) =>
            {
                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = url,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Could not open URL: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                };
            };
            messagePanel.Controls.Add(linkLabel);
        }

        // Description Label (Bottom)
        if (!string.IsNullOrEmpty(description))
        {
            Label descriptionLabel = new Label
            {
                Text = description,
                AutoSize = true,
                MaximumSize = new Size(chatPanel.Width - 100, 0), // Optional: Set a maximum width for wrapping
                ForeColor = Color.White, // Set description color to white
                Margin = new Padding(5),
                Dock = DockStyle.None,
                Location = linkLabel != null ? new Point(10, linkLabel.Bottom + 5) : new Point(0, 0) // Use linkLabel's position if it exists
            };
            messagePanel.Controls.Add(descriptionLabel);
        }

         // Thumbnail (if available)
    if (thumbnail != null)
    {
        PictureBox thumbnailBox = new PictureBox
        {
            Image = thumbnail,
            SizeMode = PictureBoxSizeMode.StretchImage,
            Width = 50, // Set width for thumbnail
            Height = 50, // Set height for thumbnail
            Margin = new Padding(5),
            Dock = DockStyle.None,
            Location = linkLabel != null ? new Point(10, linkLabel.Bottom + 5) : new Point(0, 0) // Use linkLabel's position if it exists
        };
        messagePanel.Controls.Add(thumbnailBox);
    }

        // Add the message panel to the chatPanel
        chatPanel.Controls.Add(messagePanel);
        chatPanel.ScrollControlIntoView(messagePanel); // Scroll to the latest message
    }

private void UpdateRecentQueriesSidebar()
{
    // Clear existing recent queries in the sidebar
    sidebarPanel.Controls.OfType<Label>().Where(l => l.Name == "RecentQueryLabel").ToList().ForEach(l => sidebarPanel.Controls.Remove(l));

    // Add recent queries to the sidebar
    foreach (var query in _recentQueries)
    {
        Label queryLabel = new Label
        {
            Text = query,
            AutoSize = true,
            Margin = new Padding(5),
            ForeColor = Color.White,
            BackColor = Color.FromArgb(40, 40, 40), // Background color for recent queries
            Name = "RecentQueryLabel" // Set a name for easy identification
        };
        sidebarPanel.Controls.Add(queryLabel);
    }
}
private async void searchButton_Click(object sender, EventArgs e, string query)
{
    
    // Display the user's query in the chat
    AddChatMessage($" {query}", "", "", null, true); // Pass true for user query

    // Show loading indicator
    //loadingLabel.Visible = true;

    // Hide welcome label on first search
    if (isFirstSearch)
    {
        welcomeLabel.Hide();
        isFirstSearch = false; // Set to false after the first search
    }

    try
    {
        // Perform search
        var results = await _serpApiService.SearchAsync(query);

        // Store the query and results in history
        _queryHistory.Add((query, results));
          //UpdateRecentQueriesSidebar();

        // Check if results are empty
        if (results.Count == 0)
        {
            AddChatMessage("No results found.", "", "", null, false); 
            welcomeLabel.Show(); // Show welcome label if no results
        }
        else
        {
            // Display results in chat format
            foreach (var result in results)
            {
                AddChatMessage(result.Title, result.Url, result.Description, result.ThumbnailImage, false); 
            }
        }
    }
    catch (Exception ex)
    {
        AddChatMessage($"Error: {ex.Message}", "", "", null, false); 
    } finally{
         // Hide loading indicator
        //loadingLabel.Visible = false;   
    }
}
        


    }
}
