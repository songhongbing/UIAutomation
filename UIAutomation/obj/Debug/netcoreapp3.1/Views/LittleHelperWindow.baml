   M S B A M L   `   `   `  ����G  CUIAutomation, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null?$clr-namespace:UIAutomation.ConverterUIAutomation.Converter  5      +    %UIAutomation.Views.LittleHelperWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      S OPrism.Wpf, Version=7.2.0.1422, Culture=neutral, PublicKeyToken=40ee6c3a2184dc59Y UPrism.Unity.Wpf, Version=7.2.0.1422, Culture=neutral, PublicKeyToken=40ee6c3a2184dc59&prismhttp://prismlibrary.com/   5      c	 _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bi:http://schemas.microsoft.com/expression/2010/interactivity 	 5      G
 CHandyControl, Version=2.5.0.1, Culture=neutral, PublicKeyToken=null0hc'https://handyorg.github.io/handycontrol 
 5      4	converter$clr-namespace:UIAutomation.Converter 
 5      -   5        !  Prism.Mvvm.ViewModelLocator    AutoWireViewModel$
  True��5       � AllowsTransparency$
 True��5	       � WindowStyle$
 None=�5      
������ $��75��5      $��75��6    � ShowInTaskbar$ False��5       � Topmost$
 True��6$   
 � Top$ 50��63    � Left 5   &   �� *
WindowLeft+. 	 (System.Windows.Interactivity.Interaction   Triggers 5      / 	 )System.Windows.Interactivity.EventTrigger  5   
      	EventName$ MouseDoubleClick��6   . 	 (System.Windows.Interactivity.TriggerBase	   Actions.	 5      6 	 0System.Windows.Interactivity.InvokeCommandAction  
   Command
 6$   �� *MouseDoubleCommand+5       � 	Resources 5      0   *UIAutomation.Converter.VisibilityConverter   VisibilityConverter7   1UIAutomation.Converter.ReverseVisibilityConverter  ReverseVisibilityConverter��%:   &	        &	        5   
   6(     5   
   6/   5      .��5      �� $��0q�6    �� CornerRadius$ 50��6!   ���	#FFFBFCFF._�5   
   �� .+�5      �� $��30��6   $��30��6�   
 �� Top$ 22��6�    �� Left$ 22��6�   y�6    �� *IsStartWorker+ �� 	Converter# [  �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=� 5        5      $ MouseDoubleClick��6%   .	 5        
 60   �� *MouseDoubleCommand+5      ��5      �� 5       .R�5!      ��  ��Name$ ImgLogo��6    $��30��61   $;��5/UIAutomation;component/Resources/Images/Ico/logo.ico��6<    �� Triggers 5"      P� 5#       P� Property �@J�IsMouseOver P� Value$
 True��6=   . �5$   "   ��  �� 
TargetName$ ImgLogo��6)    �� Property �@J�Opacity �� Value$	 0.7Q�6Q   5%      5'      5)      �� 5*      $��75��6   $��75��6    y�6-   �� *IsStartWorker+# [  $ TwoWay=�$ PropertyChanged=���5+      �� 5,      .R�5-      �� .+�5.      - 
 'HandyControl.Controls.CircleProgressBar    Name$ CPBInfo��63      ArcThickness$ 2Q�6m   $��20(�6   $��75��6�   $��75��6�   $��0,0,0,0q�6�    � Value 6D   �� *CircleProgressBarValue+50      �� 51       ��Name$ BtnPause��6%   $��75��6Y   $��75��6d    � Cursor$
 Hand��6p   $y�Hidden=�6~   ��67   �� *PauseCommand+ 52   "     53   &   $ MouseDoubleClick��65   .	 54   *     
 6@   �� *MouseDoubleCommand+55   '   ��57   "   �� 58   &   .R�59   *   �� $ ImgLogo��60   $��75��6A   $��75��6L    J� Opacity$	 0.8Q�6X   $A��;/UIAutomation;component/Resources/Images/Png/pause_red1.png��6f   5:   '   5<      5=       5>      P� 5?       �@J�IsMouseOver$
 True��6=   . �5@   "   �� $ CPBInfo��6)    � y�$ Hidden=�6T   �� 5A   "   $ BtnPause��6)    � y�$ Visible=�6U   5B      5D      5F      5G      5H      5I      