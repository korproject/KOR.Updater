   M S B A M L   `   `   `  ����!  KOR Updater Demo Main WPF App.    (KOR_Updater_Demo_Main_WPF_App.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   -   5          � Title$  Main App MainWindow��5   	   $	��450��6%   $	��800��62    � WindowStartupLocation$ CenterScreen=�6>   .��5      �� .@�5	   
   �� $��	Custom UI��6   .��5
      � .+�5      ��  �� Orientation$ Vertical=�6   $��Center=�64   $��Center=�6Q   .+�5      �� -   5         ��Name$ CurrentVersion��5       $��Current Version: ????��68   $��5q�6U   �� 5      -   5        $ 
NewVersion��5       $��New Version: ????��64   $��5q�6M   �� 5      -   5        $��30��5      $	��150��6)   $��Check Updates��65   $��5q�6b   �� 5      -   5         ��Name$ 
GetUpdates��5      $��30��61   $	��150��6=   $��Get Updates��6I   $��5q�6x   $y�	Collapsed=�6�   5      5      5      �� 5   
   $��Last Updates��6   .��5      �� -   5         ��Name$ LastUpdatesList��5      $��Stretch=�60    �� View 5      �� 5      .P�5      �� $��UpdateId��6)    �� Width$
 Auto��6e    �� DisplayMemberBinding 6;   �� *
UpdateId+6e   �� 5      $
��Date��6)   $
 Auto��6b    67   �� *	AddedDate+6b   �� 5      $��Version��6)   $
 Auto��6f    6:   �� *
AppVersion+6f   �� 5      $��Added Features��6)   $
 Auto��6p    6A   �� *AddedFeatures+6p   �� 5      $��Removed Features��6)   $
 Auto��6t    6C   �� *RemovedFeatures+6t   �� 5      $��Reason Code��6)   $
 Auto��6j    6>   �� *
ReasonCode+6j   �� 5      $��Reason Title��6)   $
 Auto��6l    6?   �� *ReasonTitle+6l   5      5!      5"      5#      5$      