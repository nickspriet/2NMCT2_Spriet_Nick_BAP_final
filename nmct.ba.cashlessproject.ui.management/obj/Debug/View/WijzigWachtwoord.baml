   M S B A M L   `   `   `  ����`  \nmct.ba.cashlessproject.ui.management, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullq=clr-namespace:nmct.ba.cashlessproject.ui.management.ViewModel/nmct.ba.cashlessproject.ui.management.ViewModel  5      }Cclr-namespace:nmct.ba.cashlessproject.ui.management.View.converters5nmct.ba.cashlessproject.ui.management.View.converters  5      A    ;nmct.ba.cashlessproject.ui.management.View.WijzigWachtwoord   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Lvm=clr-namespace:nmct.ba.cashlessproject.ui.management.ViewModel     5      QcCclr-namespace:nmct.ba.cashlessproject.ui.management.View.converters     5        � DataContext  5      H   Bnmct.ba.cashlessproject.ui.management.ViewModel.WijzigWachtwoordVM  5   
    � 	Resources 5      Y   Snmct.ba.cashlessproject.ui.management.View.converters.MultiBindingPasswordConverter !  MultiBindingPasswordConverter��%   &	          5   
   6*   5      .��5      � ���	#FF3C3C3C � RowDefinitions 5   
   �� 5      $��* �6   �� 5      $
��2.5* �6   �� 5      $��1* �6   �� 5      $��1* �6   �� 5      $
��0.5* �6   �� 5      $��1* �6   �� 5      $��1* �6   �� 5      $��1* �6   �� 5      $
��1.5* �6   �� 5      $��1* �6    � ColumnDefinitions 5    
   �� 5!      $
��0.9* �6   �� 5"      $��* �6   �� 5#      $��* �6   �� 5$      $
��0.9* �6   .+�5&   
   �� $��WIJZIG WACHTWOORD��6   $��0��6,     TabTitle#��[ 69   �� 5(   
   $��Gebruikersnaam:��6     GegevensLabels#��[ 6*   $��2��6R   $��1��6_   �� 5)   
    �� 
IsReadOnly .   	Loginnaam#��[ 6B   $��2��6e   $��2��6r   
�������
���������6   �� *LoginManager+6�   �� 5*   
   $��Oud wachtwoord:��6   #��[ 6*   $��3��6R   $��1��6_   G� 5+   
   -   5        $��txtOudWachtwoord��5+        Passwordbox#��[ 6.   $��3��6S   $��2��6`   �� 5,   
   $��Nieuw wachtwoord:��6   #��[ 6,   $��5��6T   $��1��6a   G� 5-   
   -   5        $��txtNieuwWachtwoord��5-      #��[ 60   $��5��6U   $��2��6b   �� 5.   
   $"��Nieuw wachtwoord bevestigen:��6   #��[ 67   $��6��6_   $��1��6l   G� 5/   
   -   5        $"��txtNieuwWachtwoordBevestigen��5/      #��[ 6:   $��6��6_   $��2��6l   �� 50   
   $��Wachtwoord wijzigen��6   $��8��6[   $��1��6h   $��2��6x   ��6/   �� *WijzigWachtwoordCommand+��51      `� 52       `� 	Converter# [  6   .1�53      ��  �� ElementName$ txtOudWachtwoord��6   �� 54      $ txtNieuwWachtwoord��6   �� 55      $" txtNieuwWachtwoordBevestigen��6   56      58      59      5:      