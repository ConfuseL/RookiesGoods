# RookiesGoods
![](https://img.shields.io/badge/language-C%23-blue.svg?style=flat-square)  ![](https://img.shields.io/badge/support-unity3D-lightgrey.svg?style=flat-square)



> 这是一个努力成为大部分Unity初学者适用的多角色物品管理插件框架，适用于单机游戏中拥有耐久度、可合成等关键字的物品数据管理需求。
>
> 学业紧张，不定时更新维护。



## 当前包含的功能

> 1. 使用XML配置角色容器（存储物品的介质：背包、仓库等）的数量以及存储种类和每个容器的容纳数量。
> 2. 使用JSON制定物品的属性，除了框架定好的固定属性，开发者可以自行配置其他属性（如伤害值、恢复值、价格等）。
> 3. 提供可视化编辑器窗口对以上的角色容器、物品属性进行直接编辑，无需编码。（2019-3-29 重大更新）
> 4. 无需关心物品存储、移除的过程以及它的数量变化，框架已经帮你实现。
> 5. 支持物品数据存档以及读档，提供读档成功的装备回调接口，开发者可以选择在此还原角色某些属性。

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

## 容器配置

### 存储种类配置

> 打开Unity3D菜单栏的RookiesGoods选项，选中StorageConfig

![](https://raw.githubusercontent.com/ConfuseL/RookiesGoods/master/PreviewImage/%E8%8F%9C%E5%8D%95%E7%AA%97%E5%8F%A3.png)

> 这时候会弹出一个编辑窗口，上面有两大项：存储种类配置、容器配置。

![](https://raw.githubusercontent.com/ConfuseL/RookiesGoods/master/PreviewImage/%E7%A9%BA%E7%9A%84%E9%85%8D%E7%BD%AE%E7%AA%97%E5%8F%A3.png)

> 我们先设置存储种类，存储种类即游戏中物品的存储类型，比如鸡肉 -> 食物，木棍 -> 武器。我们在这里需要规定游戏中到底有多少中存储类型。
>
> 默认有一个all类型，也就是通用类型。该类型不可删除。
>
> 我们点击添加存储类型按钮，在生成的输入框上，输入我们要设置的存储类型。

![](https://raw.githubusercontent.com/ConfuseL/RookiesGoods/master/PreviewImage/%E5%AD%98%E5%82%A8%E7%A7%8D%E7%B1%BB%E9%85%8D%E7%BD%AE.png)

| 其余按钮     | 作用                                                         |
| ------------ | ------------------------------------------------------------ |
| 添加存储类型 | 新增一个存储类型                                             |
| 保存存储配置 | 保存存储种类配置写入XML，这时候会自动识别原始种类是否被更改，如果被更改，会将所有与其相关的容器、物品的对应存储种类更正。如果被删除，那么所有与其相关的容器存储种类会更改为all，所有与其相关的物品存储种类会变为empty。 |

> 如果你输入了多个相同名字的存储类型，插件会自动去重，保证类型的唯一性。

### 容器配置

> 如前面所属，抽象来说就是角色或者对象可拥有的存储物品的介质，在游戏中，就是常见的背包、仓库、手持栏等等。
>
> 我们在这里要规定这个游戏中，角色/对象拥有几个容器，容器的名字，存储的类型，以及容量。
>
> 点击添加容器，在生成的数据项中，输入容器的名字，点击存储类型的下拉框，选择之前我们设置的存储类型，再设置好容量。

![](https://raw.githubusercontent.com/ConfuseL/RookiesGoods/master/PreviewImage/%E5%AD%98%E5%82%A8%E9%85%8D%E7%BD%AE%E9%94%99%E8%AF%AF%E7%A4%BA%E8%8C%83.png)

> 如上图，当出现同个名字的容器，或者是容量小于1，都会导致无法存储，这时候根据提示将其更改再保存即可。
>
> 在上图中我们设置了食物栏、装备栏（容纳装备的背包）、材料栏、手持栏（这其实才是装备栏），分别存储对应的属性，其中手持栏设置了2的容量(两只手)。

| 其余按钮         | 作用                            |
| ---------------- | ------------------------------- |
| 添加容器         | 添加一个新的容器                |
| 保存容器数据     | 将容器信息写入XML中             |
| 重新加载容器数据 | 将读取最新的XML文件，还原内容。 |

## 物品信息配置

### 正常配置

> 打开Unity3D菜单栏的RookiesGoods选项，选中GoodsConfig

![](https://raw.githubusercontent.com/ConfuseL/RookiesGoods/master/PreviewImage/%E6%89%93%E5%BC%80%E6%8A%A5%E9%94%99.png)

> 当你是第一次打开物品信息配置时，会出现上面的报错输出，可以不用理会，因为全局物品管理器在初始化的时候读取不到物品数据，由于你打开了插件提供的编辑窗口，插件会自动帮你生成json文件，但是需要你保存，否则不会写入，下一次打开依然会出现该情况。

![](https://raw.githubusercontent.com/ConfuseL/RookiesGoods/master/PreviewImage/%E7%A9%BA%E7%9A%84%E7%89%A9%E5%93%81%E9%85%8D%E7%BD%AE%E7%AA%97%E5%8F%A3.png)

> 我们第一次打开配置窗口，由于没有任何数据，编辑窗口会自动创建一个空的物品信息，包含一下内容：

| 信息项                 | 作用                                                         |
| ---------------------- | ------------------------------------------------------------ |
| id                     | 物品的唯一标识符，插件自动生成，不可更改。选择时会出现所有以配置的物品id组成的下拉框，再次点击进入该物品的信息配置。 |
| 在容器中的最大容纳量， | 物品在容器中可以存储的数量，如常见游戏背包里，物品UI右下角的数字。 |
| 名字                   | 物品的名字                                                   |
| 框架的对象类型         | 三种类型，分别是：普通物品：RookiesGoods_Composite 	、消耗品：RookiesGoods_Consumable 	、装备：RookiesGoods_SuitBase |
| 耐用度                 | 装备对象类型特有的属性，当耐用度归0时自动从背包中移除。      |
| 存储种类               | 物品在游戏中存储的种类，与上面的存储种类配置中设置挂钩，新建时物品存储种类默认为empty(空)。 |
| 精灵图                 | 物品的图片，注意，必须是Resources文件下的文件，因为框架加载图片时使用的是Resources.Load函数。 |
| 介绍                   | 物品的介绍，用于在游戏中显示。                               |
| 实际作用               | 物品对角色、任务、事件产生的作用，可用于提醒开发者。         |
| 合成表                 | 物品的合成信息，由物品id和对应数量组成，可以不设置，则表示该物品不可合成。当整个游戏只有一个物品的时候，无法提供合成表功能，因为自己不能被自己合成。 |
| 自定义属性-整形        | 开发者自己定义的整形属性，例如增加的攻击力、恢复的生命值、法力值等整数数值。支持数组，只需要重复点击<添加一个新值按钮>即可。 |
| 自定义属性-字符形      | 同上，但值的类型是字符串。                                   |
| 自定义属性-浮点形      | 同上，但值的类型为浮点行。                                   |

*为减少开发成本，省略了布尔值（可用用01代替）和JSON Object对象的自定义属性编辑，如果需要可以尝试手动编写json文件，并修改源码的解析部分。*

![](https://raw.githubusercontent.com/ConfuseL/RookiesGoods/master/PreviewImage/%E7%89%A9%E5%93%81%E9%85%8D%E7%BD%AE%E7%AA%97%E5%8F%A3.png)

> 如上图，我们新建了一个装备类型的盾牌。它的耐久度为25，即使用25次将被移除。由2个id为2的物品（木材）所合成，在自定义整形属性中，添加了名为def，值为15的整形属性，代表提供15的防御力。

| 按钮             | 作用                                                         |
| ---------------- | ------------------------------------------------------------ |
| <保存该物品信息> | 自动将信息写入JSON文件。JSON文件名为Goods.json，位于Resources\Json\下 |
| <删除该物品按钮> | 将删除这个物品，且不会再出现该ID。                           |
| <添加一个新物品> | 将直接进入新物品属性编辑窗口。                               |

### 手动配置

### 可以忽略的报错

> 1. 当你删除物品，直到只剩下一个的时候会出现这个布局错误，但不会影响实际使用。

![](https://raw.githubusercontent.com/ConfuseL/RookiesGoods/master/PreviewImage/%E5%88%A0%E9%99%A4%E9%94%99%E8%AF%AF.png)

> 2. 当你在编辑窗口打开的时候，未通过编辑窗口而是直接对文件夹中的JSON文件进行替换或者删除，也会爆出空引用的错误，这时候只需要点击重新导入所有物品信息，并且把窗口关闭，再重新打开即可。

## 查看框架API


> 这个时候你已经配置好了游戏中的容器和物品了，请查阅[角色物品数据管理器](https://github.com/ConfuseL/RookiesGoods/wiki/RookiesGoods_PlayerData)中描述的几个抽象函数配置属于你的游戏所需的角色物品管理器，同时Demo是一个很好的例子介绍如何实现适合你游戏的方式。
>
> 如果已经完成基本的配置了，仅仅是使用这个框架的话，建议阅读的顺序为[容器存储的基本单位：格子](https://github.com/ConfuseL/RookiesGoods/wiki/RookiesGoods_Grid) -> [容器](https://github.com/ConfuseL/RookiesGoods/wiki/RookiesGoods_Bag)  -> [角色物品数据管理器](https://github.com/ConfuseL/RookiesGoods/wiki/RookiesGoods_PlayerData) -> [全局数据管理器](https://github.com/ConfuseL/RookiesGoods/wiki/RookiesGoods_OverallManage) 。
