   M S B A M L   `   `   `  ����I  EEFT_Price_Tool, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:EFT_Price_ToolEFT_Price_Tool  5   	       EFT_Price_Tool.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   .localclr-namespace:EFT_Price_Tool     5   	     � Title$  EFT Price Tool��5   	   $	��400��6    $	��850��6-   
���   �.��5	      �  � ColumnDefinitions 5
   
   �� 5      $��20 �6   �� 5      $	��300 �6   �� 5      $	��300 �6   �� 5      $��* �6   �� 5      $��20 �6    � RowDefinitions 5   
   �� 5      $��20 �6   �� 5      $
��auto �6   �� 5      $
��auto �6   �� 5      $
��auto �6   �� 5      $
��auto �6   �� 5      $
��auto �6   �� 5      $��* �6   �� 5      $��20 �6   .+�5   
   �� $��	Item Name��6   $
��Left=�6$   $��Center=�6?   $��1��6Z   $��1��6j   $��24(�6w   $��	0,10,0,10q�6�   ���	#FF9A88666�   �� 5   
   $��Price��6   $
��Left=�6    $��Center=�6;   $��1��6V   $��2��6f   $��24(�6s   $��	0,10,0,10q�6�   ���	#FF9A88666�   �� 5   
   $��Price Per Slot��6   $
��Left=�6)   $��Center=�6D   $��1��6_   $��3��6o   $��24(�6|   $��	0,10,0,10q�6�   ���	#FF9A88666�   �� 5   
   $��	Item Tier��6   $
��Left=�6$   $��Center=�6?   $��1��6Z   $��4��6j   $��24(�6w   $y�Visible=�6�   $��Normal"�6�   $��Normal$�6�   
������ $��	0,10,0,10q�6�   ���	#FF9A88666�   �� 5   
   $ ��Press SHIFT + F1 to Record��6   $��Right=�65   $��Center=�6Q   $��3��6l   $��5��6|   $y�Visible=�6�   $��Normal"�6�   $��Normal$�6�   
������ $��	0,10,0,10q�6�   ���	#FF9A88666�   �� 5    
   -   5         ��Name$ tblockItemName��5       $�� ��6   $
��Wrap=�64   $��Center=�6H   $��Center=�6e   $��2��6�   $��1��6�   $��	0,10,0,10q�6�   $��24(�6�   ���	#FF9A88666�   �� 5!   
   -   5        $ tblockItemPrice��5!      $�� ��6   $
��Wrap=�65   $��Center=�6I   $��Center=�6f   $��2��6�   $��2��6�   $��24(�6�   $��	0,10,0,10q�6�   ���	#FF9A88666�   �� 5"   
   -   5        $ tblockItemPricePerSlot��5"      $�� ��6   $
��Wrap=�6<   $��Center=�6P   $��Center=�6m   $��2��6�   $��3��6�   $��24(�6�   $��	0,10,0,10q�6�   ���	#FF9A88666�   �� 5#   
   -   5        $ tblockItemTier��5#      $�� ��6   $
��Wrap=�64   $��Center=�6H   $��Center=�6e   $��2��6�   $��4��6�   $��0,10,0,1q�6�   $��24(�6�   ���	#FF9A88666�   5&      5)      