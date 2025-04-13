# DeskNote
A WPF Windows-desktop notes-tool  

This was one of my first WPF-Projects, already 10 years ago... and I still use it regularily   
  
I'll start by simply sharing it here now - later I will try to document it better   
But there's a lot of stuff in there - Well, honestly I don't remember all of the things I did :D  

Recently I realised that I followed the wrong track by storing the contents in xaml  
With xaml You can't store images and hyperlinks and more - I once made a workaround for the hyperlinks, which is still useful for scanning the text for links  
Now, as I started sharing all my little helpers, I stumbled over the image problem and found that one could simply store the content in Rtf-format  
And as a bonus the size of the Rtf-data is ~ 1/3 of the xaml-data  

# General  
  
there is a main control that manages the notes, or one may better say 'notebooks' - where each 'notebook' may consist of several pages (I store the interesting verses of my daily bible-reading there, and over the year it gets filled up to 2000 pages :D )
With the control you can create a new 'notebook', show it, hide it, delete it, send it to desktop-background...  
  
### Contextmenue for the taskbar icon  
for opened notes:    
![image](https://github.com/user-attachments/assets/a1feda45-be61-4eda-ae30-2f93f1973fc1)  
for closed notes:  
![image](https://github.com/user-attachments/assets/6427d520-8ac7-42ff-8c67-2c7953ea33f3)  
  
# Notes  

![image](https://github.com/user-attachments/assets/07d664b6-a155-4fe2-8d68-cee904b7cc8f)  

Toolbars when hovering with the mouse    
![image](https://github.com/user-attachments/assets/794d1a4b-fad7-418e-aa9f-d699796fa426)  

![image](https://github.com/user-attachments/assets/a406a91f-e422-4a32-b696-26f7051b6517)  

You can switch through the notes with the control at the bottom:  
![image](https://github.com/user-attachments/assets/49c8fa8e-f4fb-428c-9770-f615f2560d21)  
Either use the arrow-buttons, the mousewheel or the preview (when clicking on the middle of the control)  
When combining the CTRL-key and the buttons you can jump to the first or last page  

### Context menues  

  top-bar menue  
  ![image](https://github.com/user-attachments/assets/bdb630a7-ca3a-4a76-8e40-fc353924c213)  
![image](https://github.com/user-attachments/assets/9f7dbd50-2446-42cc-8dd6-51e316284b87)  

  text menue 
  ![image](https://github.com/user-attachments/assets/de1f79b1-62ba-4fea-aa7b-7134e0e12541)  



# Settings  

The settings can be opened either with the top-bar context menue or by double-clicking on the top-bar and holding the CTRL-key   
On the first tab one can configure the background-color and additional text formattings  
The background-color can only be adjusted when 'image' is not selected for background

![image](https://github.com/user-attachments/assets/bf0e5f22-f390-478d-84c2-318982fe0a6e)  

When 'image' is selected for background one can adjust the different colorvalues on this tab  
One may save the settings as a preset  
![image](https://github.com/user-attachments/assets/88d0e3f5-ac81-46ac-9059-695577f6b4e0)  
Though one can load huge pictures as background you shouldn't do that as it unnecessarily eats up memory - better use a smaller copy  
On the last tab on can add different fonts as favourites - they then will be displayed on top of the font list  
![image](https://github.com/user-attachments/assets/a2c17768-46ee-460a-aeef-b9e5029fcbad)  
![image](https://github.com/user-attachments/assets/384f81a4-4085-4921-93dc-d6a52ffbe2be)  

The default settings are used for new pages   



