# RookiesGoods
![](https://img.shields.io/badge/language-C%23-blue.svg?style=flat-square)  ![](https://img.shields.io/badge/support-unity3D-lightgrey.svg?style=flat-square)



> 这是一个努力成为大部分Unity初学者适用的多角色物品管理框架，适用于游戏中拥有耐久度、可合成等关键字的物品数据管理需求。
>
> 学业紧张，不定时更新维护。



## 当前包含的功能

> 1. 可以通过配置文件配置角色容器（存储物品的介质：背包、仓库等）的数量以及存储种类和每个容器的容纳数量。
> 2. 使用JSON制定物品的属性，除了框架定好的固定属性，开发者可以自行配置其他属性（如伤害值、恢复值、价格等）。
> 3. 无需关心物品存储、移除的过程以及它的数量变化，框架已经帮你实现。
> 4. 支持物品数据存档以及读档，提供读档成功的装备回调接口，开发者可以选择在此还原角色某些属性。（最近更新）

## 当前支持的物品类型

###### 普通物品：

> 可合成，如果可以成功合成，会自动从容器中移除所需物品和对应数量，并生成该物品。

###### 消耗品：

> 可合成，合成机制同上，拥有消耗机制，调用使用接口会自动从容器中移除，在此之前，你可以获取它的属性进行相应的操作。

###### 装备：

> 可合成，合成机制同上，可拥有耐久度机制，在装备的条件下调用使用接口会自动减少耐久度，直到为0，此时会自动从容器中移除。支持无限耐久度。



## Simple Demo Screenshot

![](https://raw.githubusercontent.com/ConfuseL/RookiesGoods/master/PreviewImage/%E7%AE%80%E5%8D%95DEMO%E6%BC%94%E7%A4%BA.gif)

<center>Demo的UI比较简陋,，仅为展示框架功能</center>

![](https://raw.githubusercontent.com/ConfuseL/RookiesGoods/master/PreviewImage/%E8%80%90%E4%B9%85%E5%BA%A6%E5%BD%92%E9%9B%B6%E6%BC%94%E7%A4%BA.gif)

<center>耐久度归零自动移除</center>

![](https://raw.githubusercontent.com/ConfuseL/RookiesGoods/master/PreviewImage/%E5%AD%98%E6%A1%A3%E8%AF%BB%E6%A1%A3%E6%BC%94%E7%A4%BA.gif)

<center>物品数据的存档读档</center>

## Demo使用方法

> 下载Demo压缩包，将Asset文件夹内容放入一个新项目的Asset下即可。打开Scenes文件夹下的demo场景并运行。
>
> 因为Demo文件夹中以包含RookiesGoods文件，因此两者不需要同时下载。



## 下一步功能

> 1. 实现配置多角色的容器，当前仅为所有角色使用同种配置。
> 2. 增加时效度物品类，也就是在生成之后的规定时间结束就作废的物品，参考饥荒的食物。



# 配置和使用


> 内容以移步至本库[wiki](https://github.com/ConfuseL/RookiesGoods/wiki)
