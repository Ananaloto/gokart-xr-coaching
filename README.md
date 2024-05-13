# gokart-coaching
This package focuses on exploring the improvement of coaching experience for inexperienced drivers in karting with more comprehensive feedback. This involves the implementation of an immersive coaching application designed for use with Mixed Reality headsets. This project is done in IDSC-Frazzoli Groupe at ETH Zurich.

This package’s contributions include developing an integrated hardware and software framework for head-worn displays with the go-kart platform, implementing head-worn display localization, developing applications for virtual feedback, and validating the effectiveness of immersive coaching through Mixed Reality technology.

![final_demo](https://github.com/idsc-frazzoli/gokart-coaching/blob/galactic-coaching/Images/final_demo.gif)

The main compositions realized in the application:
1. A virtual red cylinder anchor to prove the reliability of localization.
2. A logger to display useful information working as a console interface.
3. The ideal path calculated from MPCC to indicate real-time optimal driving path.

## Hardware
1. Head-worn display (HWD): **Magic Leap 2**
2. Onboard camera: **World Cameras**
3. Tag to help with localization: **April Tag 25h9**. ID: 7 and 22. Size: 10cm x 10cm.

## Software
The main software used are Unity and Magic Leap Hub. The versions of the packages are listed as below.
| Package Name | Version |
| -------- | ------- |
| ML C SDK | 1.5.0 |
| ML C SDK CMake Support | 1.0.6.0 |
| Magic Leap Application Simulator for Unity | 3.8.0.20231208 |
| Unity Editor | 2022.3.17f1 |
| Unity Package | 2.0.0 |
| Unity MRTK | 1.12.1-v1 |

## Ways to Run Applications
There are two possible ways to run applications created:
1. **Use Application Simulator in Unity on PC:**
    - Advantages: Faster to build, debug info displayed in Unity console, convenient to debug.
    - Disadvantages: Lower image quality, significant delay, High GPU requirements for PC, needs to be always connected to PC.
2. **Built-in application on ML2:**
    - Advantages: High and consistent image quality, lower delay, runs independently without connecting to PC.
    - Disadvantages: Takes longer to build, needs to rebuild the application after each code changing, difficult to debug without console interface.

Given their properties, it is recommanded to code and debug with the first way using Application Simulator, and to only test with the second way using the built-in application once there is a working version.

## Ways to Connect ML2 and Go-kart
As we have two ways to run applications, there are also two available options to realize connection between ML2 and go-kart:
1. **Connect ML2 and go-kart with the help of PC, run code with Application Simulator in Unity.**
2. **Connect ML2 directly to go-kart, run code with built-in Application on ML2.**
![connection](https://github.com/idsc-frazzoli/gokart-coaching/blob/galactic-coaching/Images/2_ways_connection.png)

Their advantages and disadvantages are the same as before. Furthermore, the second connection means that a PC needs not to be installed on the go-kart when driving, users can use the lanyard of ML2 to hang it around their necks or attach it somewhere on the go-kart, which is convenient and safer. This is also the way to use in practical case of coaching.
