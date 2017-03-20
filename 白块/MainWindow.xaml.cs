using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Media;
namespace 白块
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Welcome();
            InitClassicTimer();
            PlayBackground();
            
        }
        Canvas canvas_arc;//街机模式的canvas
        Grid gd_label;
        Random r = new Random();
        Label l,lb;
        int m,n;
        List<Label> list_label = new List<Label>();//经典模式
        List<Label>list_label1=new List<Label>();//街机模式
        Button bt_start;//经典模式的按钮
        Button bt_arcade;//街机模式的按钮
        Button bt_help;//游戏帮助的按钮
        Button bt_tip;//我知道了按钮
        Button bt_Over;//游戏结束时按钮
        Label lb_tip;//小提示的label
        Label use_time;//记录已用时间
        Label label_Arc_score;//街机模式记录分数的Label
        Label label_get_score;//经典模式所获得分数
        DispatcherTimer dt_classic_timer;//控制经典模式的timer
        DispatcherTimer dt_Arcade_create;//控制街机模式产生黑白块的timer
        DispatcherTimer dt_Arcate_move;//控制街机模式移动黑白块的timer
        DateTime  dt_second;
        DateTime dt_mil_second;
        TimeSpan ts_second;
        TimeSpan ts_mil_second;
        int score;//经典模式所获得的分数
        int score1=0;//街机模式中分数
        SoundPlayer sp_background = new SoundPlayer();
        bool IsStart = false;
        bool IsArc = false;
        SoundPlayer sp_click_true;
        SoundPlayer sp_click_false;
        //背景音乐的播放
        void PlayBackground()
        {
            sp_background = new SoundPlayer(@"Resources\background.wav");
            sp_background.PlayLooping();
         
            
        }
        //点击正确时播放的声音
        //void PlayClickTrue()
        //{
        //    sp_click_true = new SoundPlayer(@"Resources\click_true.wav");
        //    sp_click_true.Load();
        //    sp_click_true.Play();
        //}
        //void PlayClickFalse()
        //{
        //    sp_click_false = new SoundPlayer(@"Resources\click_false.wav");
        //    sp_click_false.Load();
        //    sp_click_false.Play();
        //}
        void Welcome()
        {
            border_back.Background = new SolidColorBrush(Colors.LawnGreen);

            //经典模式的按钮
            bt_start = new Button();
            bt_start.Width = 80;
            bt_start.Height = 30;
            bt_start.Content = "经典模式";
            bt_start.Background = new SolidColorBrush(Colors.Silver);
            Canvas.SetLeft(bt_start,Canvas_Back.Width/2-bt_start.Width/2);
            Canvas.SetTop(bt_start,Canvas_Back.Height/2-bt_start.Height/2);
            Canvas_Back.Children.Add(bt_start);
            bt_start.Click += new RoutedEventHandler(bt_start_Click);
            //街机模式的按钮
            bt_arcade = new Button();
            bt_arcade.Width = 80;
            bt_arcade.Height = 30;
            bt_arcade.Content = "街机模式";
            bt_arcade.Background = new SolidColorBrush(Colors.SlateBlue);
            Canvas.SetLeft(bt_arcade,Canvas_Back.Width/2-bt_arcade.Width/2);
            Canvas.SetTop(bt_arcade,Canvas_Back.Height/2-bt_arcade.Height/2+30);
            Canvas_Back.Children.Add(bt_arcade);
            bt_arcade.Click += new RoutedEventHandler(bt_arcade_Click);
            //游戏帮助的按钮
            bt_help = new Button();
            bt_help.Width = 80;
            bt_help.Height = 30;
            bt_help.Content = "游戏帮助";
            bt_help.Background = new SolidColorBrush(Colors.HotPink);
            Canvas.SetLeft(bt_help,Canvas_Back.Width/2-bt_help.Width/2);
            Canvas.SetTop(bt_help,Canvas.GetTop(bt_arcade)+30);
            Canvas_Back.Children.Add(bt_help);
            bt_help.Click += new RoutedEventHandler(bt_help_Click);


        }
        //街机模式按钮的点击事件
        void bt_arcade_Click(object sender, RoutedEventArgs e)
        {
            IsArc = true;
            InitArcCanvas();
            InitArcadeT();
            border_back.Background = new SolidColorBrush(Colors.LightYellow);
            Canvas_Back.Children.Remove(bt_help);
            Canvas_Back.Children.Remove(bt_start);
            Canvas_Back.Children.Remove(bt_arcade);
            dt_Arcate_move.Start();
            dt_Arcade_create.Start();

        }
        //初始化街机模式中两个timer
        void InitArcadeT()
        {
            if(dt_Arcade_create==null)
            {
             dt_Arcade_create = new DispatcherTimer();
             dt_Arcade_create.Interval = TimeSpan.FromMilliseconds(900);
            }
            dt_Arcade_create.Tick += new EventHandler(dt_Arcade_create_Tick);
            if(dt_Arcate_move==null)
            {
                dt_Arcate_move = new DispatcherTimer();
                dt_Arcate_move.Interval = TimeSpan.FromMilliseconds(30);
            }
            dt_Arcate_move.Tick += new EventHandler(dt_Arcate_move_Tick);
        }
        //初始化街机模式的Canvas
        void InitArcCanvas()
        {
            canvas_arc = new Canvas();
            canvas_arc.Width = 300;
            canvas_arc.Height = 570;
            canvas_arc.Opacity = 0.8;
            canvas_arc.Background = new SolidColorBrush(Colors.HotPink);
            Canvas.SetLeft(canvas_arc,Canvas_Back.Width/2-canvas_arc.Width/2);
            Canvas.SetTop(canvas_arc,0);
            Canvas_Back.Children.Add(canvas_arc);
            //new那个记录分数的label
            label_Arc_score = new Label();
            label_Arc_score.Height = 35;
            label_Arc_score.Width = 130;
            label_Arc_score.Opacity = 0.8;
            label_Arc_score.Background = new SolidColorBrush(Colors.BlueViolet);
            label_Arc_score.Content = "得分:"+score1;
            Canvas.SetLeft(label_Arc_score,canvas_arc.Width/2-label_Arc_score.Width/2);
            Canvas.SetTop(label_Arc_score,80);
            Canvas_Back.Children.Add(label_Arc_score);
        }
        //街机模式中控制产生黑白块的timer事件
        void dt_Arcade_create_Tick(object sender, EventArgs e)
        {
            m = r.Next(0,4);
            n = r.Next(0,4);
            for (int i = 0; i < 4;i++ )
            {
                lb = new Label();
                lb.Width = 70;
                lb.Height = 90;
                lb.Background = new SolidColorBrush(Colors.White);
                if(i==m||i==n)
                {
                    lb.Background = new SolidColorBrush(Colors.Black);
                }
                lb.MouseLeftButtonDown += new MouseButtonEventHandler(lb_MouseLeftButtonDown);
                list_label1.Add(lb);
                
                Canvas.SetLeft(lb,i*(lb.Width+5));
                Canvas.SetTop(lb,-90);
                canvas_arc.Children.Add(lb);
                    
                
            }
           
        }

       
        //街机模式中控制移动黑白块的timer事件
        void dt_Arcate_move_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < list_label1.Count;i++ )
            {
                Canvas.SetTop(list_label1[i],Canvas.GetTop(list_label1[i])+3);
                //判断黑白块是否到大屏幕看不到的地方
                if(Canvas.GetTop(list_label1[i])>canvas_arc.Height)
                {
                    if (list_label1[i].Background.ToString() == "#FF000000")//说明掉下去的是黑块，则游戏结束
                    {
                        dt_Arcade_create.Stop();
                        dt_Arcate_move.Stop();
                        GameOver();
                    }
                    else  //说明掉下去的是白块，这释放它
                    {
                        canvas_arc.Children.Remove(list_label1[i]);
                    }
                }
            }

        }
        //街机模式中黑白块的点击事件,并不是只能点击最后一行的黑白块，这与经典模式不同，所以不需要tag值
        Label lb_click;
        void lb_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            lb_click = sender as Label;
            if (lb_click.Background.ToString() == "#FF000000")
            {
                lb_click.Background = new SolidColorBrush(Colors.White);
                score1++;
                label_Arc_score.Content = "得分:"+score1;
            }
            else
            {
                GameOver();
            }
        }
        //游戏结束方法
        void GameOver()
        {
           if(dt_Arcate_move.IsEnabled==true)
           {
               dt_Arcate_move.Stop();
           }
           if(dt_classic_timer.IsEnabled==true)
           {
               dt_classic_timer.Stop();
           }
           if(dt_Arcade_create.IsEnabled==true)
           {
               dt_Arcade_create.Stop();
           }
            //new游戏结束按钮
            bt_Over = new Button();
            bt_Over.Width = 140;
            bt_Over.Height = 35;
            bt_Over.Background = new SolidColorBrush(Colors.LemonChiffon);
            bt_Over.Content = "很遗憾，你已经挂了";
            bt_Over.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
            Canvas.SetLeft(bt_Over,Canvas_Back.Width/2-bt_Over.Width/2);
            Canvas.SetTop(bt_Over,Canvas_Back.Height/2-bt_Over.Height/2);
            bt_Over.Click += new RoutedEventHandler(bt_Over_Click);
            Canvas_Back.Children.Add(bt_Over);
        }
        //游戏结束按钮点击事件
        void bt_Over_Click(object sender, RoutedEventArgs e)
        {
            Canvas_Back.Children.RemoveRange(4,Canvas_Back.Children.Count);
            Welcome();
        }
        //bt_help按钮的点击事件
        void bt_help_Click(object sender, RoutedEventArgs e)
        {
            Canvas_Back.Children.RemoveRange(4,Canvas_Back.Children.Count);
            lb_tip = new Label();
            lb_tip.Width = 200;
            lb_tip.Height = 100;
            lb_tip.Background = new LinearGradientBrush(Colors.Honeydew,Colors.LightPink,30);
            lb_tip.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
            lb_tip.Content = "游戏规则：\r\n 游戏中只能点击黑色的方块\r\n不能点击白色的，就这样\r\n点中白色的，后果自已承担";
            Canvas.SetLeft(lb_tip,Canvas_Back.Width/2-lb_tip.Width/2);
            Canvas.SetTop(lb_tip,Canvas_Back.Height/2-lb_tip.Height/2);
            Canvas_Back.Children.Add(lb_tip);

            bt_tip = new Button();
            bt_tip.Width = 80;
            bt_tip.Height = 30;
            bt_tip.Background = new SolidColorBrush(Colors.Honeydew);
            bt_tip.Content = "我知道了";
            Canvas.SetLeft(bt_tip,Canvas_Back.Width/2-bt_help.Width/2);
            Canvas.SetTop(bt_tip,Canvas.GetTop(lb_tip)+lb_tip.Height);
            Canvas_Back.Children.Add(bt_tip);
            bt_tip.Click += new RoutedEventHandler(bt_tip_Click);
            
        }
        //I Know按钮点击事件
        void bt_tip_Click(object sender, RoutedEventArgs e)
        {
            Canvas_Back.Children.Remove(bt_tip);
            Canvas_Back.Children.Remove(lb_tip);
            Welcome();
        }
        //初始化经典模式的timer
        void InitClassicTimer()
        {
            if(dt_classic_timer==null)
            {
                dt_classic_timer = new DispatcherTimer();
                dt_classic_timer.Interval = TimeSpan.FromMilliseconds(20);
            }
            dt_classic_timer.Tick += new EventHandler(dt_classic_timer_Tick);
        }
        //经典模式timer的事件
        void dt_classic_timer_Tick(object sender, EventArgs e)
        {
            
            ts_second = DateTime.Now - dt_second;
         
            ts_mil_second = DateTime.Now - dt_mil_second;
            use_time.Content = "当前已用时间："+ts_second.Seconds+"'"+ts_mil_second.Milliseconds;
            label_get_score.Content = "得分："+score;
        }

        
        //经典模式按钮 的点击事件
        void bt_start_Click(object sender, RoutedEventArgs e)
        {
            IsStart = true;
            Canvas_Back.Children.Remove(bt_start);
            Canvas_Back.Children.Remove(bt_arcade);
            border_back.Background = new SolidColorBrush(Colors.LimeGreen);
            //new那个记录时间的label
            use_time = new Label();
            use_time.Width = 200;
            use_time.Height = 30;
            use_time.Background = new SolidColorBrush(Colors.Red);
            Canvas.SetTop(use_time,Canvas.GetTop(border_back1)+35);
            Canvas.SetLeft(use_time,250);
            Canvas_Back.Children.Add(use_time);
            //new那个记录分数的label
            label_get_score = new Label();
            label_get_score.Width = 100;
            label_get_score.Height = 30;
            label_get_score.Background = new SolidColorBrush(Colors.Blue);
            Canvas.SetLeft(label_get_score,100);
            Canvas.SetTop(label_get_score,Canvas.GetTop(use_time));
            Canvas_Back.Children.Add(label_get_score);
            InitGrid();
            dt_second = DateTime.Now;
            dt_mil_second = DateTime.Now;
            dt_classic_timer.Start();
           
           
           
        }
        
        //初始化经典模式的Grid.
        void InitGrid()
        {
            gd_label = new Grid();
            gd_label.Width = 300;
            gd_label.Height = 380;
            //将grid分为四行四列。
            for (int i = 0; i < 4;i++ )
            {
                gd_label.RowDefinitions.Add(new RowDefinition());
                gd_label.ColumnDefinitions.Add(new ColumnDefinition());
            }

            Canvas_Back.Children.Add(gd_label);
            Canvas.SetLeft(gd_label,Canvas_Back.Width/2-gd_label.Width/2);
            Canvas.SetTop(gd_label,95);


            for (int i = 0; i < 4; i++)
            {
              
                    for (int j = 0; j < 4; j++)
                    {
                        l = new Label();
                        l.Width = 70;
                        l.Height = 90;
                        l.Background = new SolidColorBrush(Colors.White);
                        l.SetValue(Grid.RowProperty, i);
                        l.SetValue(Grid.ColumnProperty, j);
                        l.MouseLeftButtonDown += new MouseButtonEventHandler(l_MouseLeftButtonDown);
                        //l.MouseLeftButtonDown += new MouseButtonEventHandler(l_MouseLeftButtonDown);
                        l.Tag = i * 4 + j;
                        list_label.Add(l);
                        gd_label.Children.Add(l);
                    }
                    //下面保证每行至少有一个黑块，最多两个黑块
                    m = r.Next(0, 4);
                    list_label[i * 4 + m].Background = new SolidColorBrush(Colors.Black);
                    
                    m = r.Next(0, 4);

                    list_label[i * 4 + m].Background = new SolidColorBrush(Colors.Black);
                    
               
            }
         
        }
        //经典模式的点击事件
        Label lab_click;
        void l_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            lab_click = sender as Label;
            //messagebox.show(lab_click.background.tostring());
            int tag = int.Parse(lab_click.Tag.ToString());
            if (tag > 11 && tag < 16)
            {
                if (lab_click.Background.ToString() == "#FF000000")
                {
                    lab_click.Background = new SolidColorBrush(Colors.White);
                    //PlayClickTrue();
                    //PlayBackground();
                }
                else
                {
                    GameOver();
                }
            }
            if (list_label[12].Background.ToString() == "#FFFFFFFF" && list_label[13].Background.ToString() == "#FFFFFFFF" && list_label[14].Background.ToString() == "#FFFFFFFF" && list_label[15].Background.ToString() == "#FFFFFFFF")
            {
                Changelabel();
            }
            score++;
        }
        //经典模式中产生黑白块的方法
        void Changelabel()
        {

            for (int i = list_label.Count - 1; i > 3; i--)
            {
                list_label[i].Background = list_label[i - 4].Background;
            }
            for (int i = 0; i < 4; i++)
            {
                list_label[i].Background = new SolidColorBrush(Colors.White);
            }
            m = r.Next(0, 4);
            list_label[m].Background = new SolidColorBrush(Colors.Black);
            m = r.Next(0, 4);
            list_label[m].Background = new SolidColorBrush(Colors.Black);


        }
        //取消按钮
        private void img_cancle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Canvas_Back.Children.RemoveRange(4,Canvas_Back.Children.Count);
            label_get_score.Content = 0;
            Welcome();
          
        }
        //关闭按钮
        private void img_close_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

      

    }
}
