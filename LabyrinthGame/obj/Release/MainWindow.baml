   M S B A M L   `   `   `  ���� H  DLabyrinthGame, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:LabyrinthGameLabyrinthGame      LabyrinthGame.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     -localclr-namespace:LabyrinthGame     -     � Title$  Игра��$
��1000��$
��1600�� � WindowStartupLocation$ CenterScreen=����	#FFDDDDDD � 
ResizeMode$ CanMinimize=�.���� -    ��Name$ mainStackPanel��$��Center=�$��0q�$��Center=�$y�Visible=�.+��� -   $ menuStackPanel�� �� Orientation$ Vertical=�$y�Visible=�$	��800��.+�]� -    ]�Name$ playerNameDockPanel��$��0, 40q���. .+��� -    ��Name$ enterNameTextBlock�� �� Padding$ 4q�$%��Введите ваше имя:��$��21.333(��� -    ��Name$ playerNameTextBox��$��4q�$��10, 0, 0, 0q�$��
Игрок��$��21.333(�$
��Bold"�	 �� 	MaxLength$	 30���� -   
 ��Name$
 newGameButton��$��60��$��0, 40q�$��Новая игра��$��29.333(��� -   $
 resultsButton��$��60��$��0, 20q�$!��Таблица итогов��$��29.333(��� -	   $ resStackPanel��$ Vertical=�$y�	Collapsed=�.+�]� -
   $ resultsMenuDockPanel����. .+��� -   $
 
backButton��$
��Left=�$��В меню��$��20(�$	��200��$��40���� -   $
 clearButton��$��Right=�$%��Очистить таблицу��$��20(�$	��200��$��40���� -   $ resultsInfoTextBlock��$"��Таблица итогов:��$��Center=�$��22(� �� TextAlignment$ Center=��� -    ��Name$ resultsListView��$
��1000��$��0, 20q�$	��800��$��Disabled=�$��20(�$��Visible=� �� View ��  �� AllowsColumnReorder .  .P���  �� Width$	 180�� �� DisplayMemberBinding �� *
PlayerName+���� .��Имя игрока�� $	 180�� �� *
CoinsCount+���� .��Собрано монет�� $	 180�� �� *Time+���� .��Время игры�� $	 180�� �� *Date+���� .��Дата запуска�� $	 220�� �� *EndingReason+���� .��%#Причина завершения�� -   $ gameStackPanel��$y�	Collapsed=�.+�]� -   $ gameMeunuDockPanel����. .+��� -   $
 toMenuButton��$
��Left=�$	��270��$��40��$��20(�$2��,Завершить и выйти в меню��|�.  �� -   $
 
anewButton��$��Right=�$	��270��$��40��$��20(�$��Начать заново��|�.  �� -   $ gameInfoTextBlock��$��Center=�$ Center=�$��22(�$g��aСоберите как можно больше монет, не попадитесь зомби!��]� -   $ statDockPanel��$��0, 40, 0, 0q���. ���� *ActualWidth+ �� ElementName$ 
gameCanvas�� �� Mode$ OneWay=�.+��� -   $ coinsCountTextBlock��$
��Left=�$	��270��$ 4q�$��21.333(�$
 Left=�$��Монет: 0���� -   $ timeTextBlock��$��Right=�$	��270��$ 4q�$��21.333(�$ Right=��� -   $ playerNameTextBlock��$ 4q�$��21.333(�$ Center=��� -    ��Name$ 
gameCanvas��|�.  K� ZIndex$ 0��