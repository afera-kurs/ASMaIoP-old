   M S B A M L   `   `   `  ????B  >ASMaIoP, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!clr-namespace:ASMaIoPASMaIoP  5   	       ASMaIoP.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   'localclr-namespace:ASMaIoP     5   	   -   5          ?Name$  mainWnd??5   	    ? Title$ ASMaIoP??5   	   $	??666??6   $	??365??6%    ? WindowStartupLocation$ CenterScreen=?63    ? 	Resources 5	         MyImageSource ?? 	UriSource??%)   &	        ?? 5
   
   $ 1.png??6,   5      .??5      ? -   5         ?Name$ MainGrid??5      $
??auto??6   $
??auto??6*    ? ColumnDefinitions 5   
   ?? 5      $	??16* ?6   ?? 5      $??9* ?6   ??5   
   ?? 5      -   5        imgbrush?? j? Viewbox$ -1.9,-1.9,2.9,2.9?5   +    ?? ImageSource# ?   6G   	 ? RowDefinitions	 5   
   ?? 5      $??20 ?6   6*   ?? 5      $	??93* ?6   6+   ?? 5      $	??71* ?6   6+   .+?5   
   o? -   5        
 o?Name$
 MainMenu??5      $??18??6!   $??Center=?6-   ${?False??6H   $??2??6Z   .@?5      m? $??Профиль??6   .@?5      m? -   5        $??Свой профиль??5      6Q   ?? 5      m? 5      -   5         m?Name$ CreateProfile??5   4   $#??Создать профиль??6   6n   5      m? 5      $??Поручение??6   .@?5       m? -   5        $%??Список поручений??5       6S   m? 5!      -   5        $ 
CreateList??5!      $'??!Создать поручение??6/   6j   ?? 5"      5#      m? 5$      $??Инвентарь??6   .@?5%      m? -	   5        $%??Всё оборудование??5%      6W   m? 5&      -
   5        $/??)Добавить оборудование??5&      6[   ?? 5'      5(      5)      ?? 5*   
   -   5         ??Name$ imgbox??5*      $??1??6   6/   ?? 5+   
   -   5         ??Name$ ContentView??5+      $
??auto??6.   $
??auto??6;   $??2??6I   $??2??6]   $??1??6n   $
??Left=?6{   $	??Top=?5,      5-      5.      