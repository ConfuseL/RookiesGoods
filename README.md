# RookiesGoods
![](https://img.shields.io/badge/language-c#-orange.svg?style=flat-square)![](https://img.shields.io/badge/support-unity3D-lightgrey.svg?style=flat-square)



> 这是一个努力成为大部分Unity初学者适用的多角色物品管理框架，适用于游戏中拥有耐久度、可合成等关键字的物品数据管理需求。
>
> 学业紧张，不定时更新维护。



## 当前包含的功能

> 1. 可以指定角色容器的数量以及存储种类和每个容器的容纳数量。
> 2. 使用JSON制定物品的属性，除了框架定好的固定属性，开发者可以自行配置其他属性。
> 3. 无需关心物品存储、移除的过程以及它的数量变化。



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



## Demo使用方法

> 下载Demo文件夹，将其子文件夹放入一个新项目的Asset下即可。打开Scenes文件夹下的demo场景并运行。
>
> 因为Demo文件夹中以包含RookiesGoods文件，因此两者不需要同时下载。



## 下一步功能

> 1. 完善多角色物品数据的存储和读取。
> 2. 实现配置多角色的容器，当前仅为所有角色使用同种配置。
> 3. 增加时效度物品类，也就是在生成之后的规定时间结束就作废的物品，参考饥荒的食物。



# 配置和使用



## 下载

> 下载RookiesGoods文件夹至Asset目录下即可



## 配置物品

> 新建一个编码为UTF-8的.json文件，进行适合您设计好物品的配置。
> 并将它放置在Resources下的任意位置，如果没有Resources文件夹，请自行创建。



#### 规定模板：

```
[{
		"id": 1
		"classType": "",
		"itemType": "",
		"name": "",
		"maxNum": 1,
		"spritePath": "",
		"intro": "",
		"effect": "",
		"durability": 0
		"compositeArray": [],
		"specialProperty": {}
	}
	]
```

##### 键值对说明：

> "id": 物品的唯一标识符,
> "classType": 物品对应的框架类，只能为以下一种：
> ​		1.普通物品：RookiesGoods_Composite
> ​		2.消耗品：RookiesGoods_Consumable
> ​		3.装备：RookiesGoods_SuitBase
> "itemType": 物品在游戏中的类型，例如武器、食物、药物等
> "name":名字
> "maxNum":在容器里一个格子最多存储的个数
> "spritePath": 图片地址
> "intro": 用于在游戏中展示物品的介绍，可以省略不写，仅""即可
> "effect": 实际效果说明，可以省略不写，仅""即可
> "durability": 0,耐久度，仅在classType为装备类RookiesGoods_SuitBase时有效，为0代表无限耐久
> "compositeArray": []合成数组，第一位为所需物品id，第二位为数量，第三位为下一个所需物品的id，第四位为数量，以此类推
> "specialProperty": {}自定义的属性

#### 例子：

```JSON
[{
		"id": 1,
		"classType": "RookiesGoods_SuitBase",
		"itemType": "weapon",
		"name": "木棒",
		"maxNum": 1,
		"spritePath": "/Sprites/1.png",
		"intro": "枯枝制作而成，看起来很脆",
		"effect": "提高15点攻击力",
		"compositeArray": [2, 1],
		"durability": 20,
		"specialproperty": {
			"atk": 15
		}
	},
	{
		"id": 2,
		"classType": "RookiesGoods_GoodsBase",
		"itemType": "material",
		"name": "枯枝",
		"maxNum": 5,
		"spritePath": "/Sprites/2.png",
		"intro": "从大树下捡到",
		"effect": "基础合成物",
		"compositeArray": [],
		"durability": 0,
		"specialProperty": {}
	}
]
```

##### 说明：

> id为1的木棒，对应的类是框架中的装备类，在游戏中它属于武器，在所有容器中，一个格子只能存放一个木棒，它的耐久度是20，由1个id为2的物品组成，拥有一个特殊属性：atk -> 攻击力 值是15。
> id为2的枯枝，对应的类是框架中的普通物品类，在游戏中它属于材料，在所有容器中，一个格子只能放5个枯枝，因为它不是装备类，因此不具有耐久度属性，不能被合成，没有特殊属性。



## 配置容器

> 打开RookiesGoods/Config/RookiesGoods_Config.xml
> 配置json文件的路径以及进行适合您游戏角色拥有的容器配置。



#### 规定模板：

```xml
<?xml version="1.0" encoding="utf-8"?>

<RookiesGoods_Config>
  <JsonPath>
    <Path>Json/Goods</Path>
  </JsonPath>
<BagMenu>
  <BagName>
    <Volume>10</Volume>
    <Type>material</Type>
  </BagName>
</BagMenu>
</RookiesGoods_Config>

```

##### 节点说明：

> <RookiesGoods_Config>配置文件头节点
> <JsonPath>//子节点，存储物品json文件的路径
>   <Path>由于读取时使用Resource.load读取，因此文件路径在编辑器中为Asset/Resource/xxx/.../yyy.json 中的xxx/.../yyy ，无需后缀名
> <BagMenu>//子节点，存储容器配置
> ​	<BagName>//容器的名字 自定义
>   <Volume>//存储的容量，也就是格子数
>   <Type>//存储的类型，注意，是游戏中的种类，与物品json文件中的itemType对应，而不是框架的类

#### 例子：

```xml
<?xml version="1.0" encoding="utf-8"?>

<RookiesGoods_Config>
 <JsonPath>
    <Path>Json/Goods</Path>
  </JsonPath>
<BagMenu>
  <HandBag>
    <Volume>2</Volume>
    <Type>weapon</Type>
  </HandBag>
  <MaterialBag>
    <Volume>30</Volume>
    <Type>material</Type>
  </MaterialBag>
</BagMenu>
</RookiesGoods_Config>
```

##### 说明：

> 定义了物品json的路径： Json/Goods   ， 而在编辑器中，路径为Asset/Resource/Json/Goods.json
>
> 创建了两个容器：
>
> 一个叫做HandBag，可以理解为手持栏，它存储游戏中的武器类型物品，容量为2，即两个格子
> 另一个叫MaterialBag，可以理解为材料包，它存储游戏中的材料类型物品，容量为30，即三十个格子



## 为角色添加物品数据管理脚本

> 创建属于您游戏中某一个角色的数据管理脚本，并继承于RookiesGoods_PlayerData.cs。
>
> 在这个管理器脚本中，您必须要实现以下几个抽象函数。



### 1.合成成功回执

> OnComposeSuccessful(RookiesGoods_Composite composite)

**功能说明：**

> 当合成物品成功时会调用这个函数，可以在这个函数中，对合成物品进行一些操作，比如存入对应的容器、或者转换成对应类型进行装备、使用等。

**例子1：**

```C#
   public override void OnComposeSuccessful(RookiesGoods_Composite composite)
    {
        //把物品自动存入适配的容器
        AutoAdd2Bag(composite);
    }
```

**例子2：**

```C#
    public override void OnComposeSuccessful(RookiesGoods_Composite composite)
    {
        //判断这个合成物的种类是不是武器 并且 存储武器的容器是否还有空位
        if (composite.ItemType == "equipment" && TryGetBag("equipment").EmptyGrids > 0)
        {
            //如果成立 把该合成物添加到存储装备的容器中
            TryGetBag("equipment").Add(composite);
            //已知该物品继承于装备类，强制转换为装备类，调用装备函数装备给当前角色。
            ((RookiesGoods_SuitBase)composite).SuitUp(this);
        }
        else
            //否则自动添加于适配的容器
            AutoAdd2Bag(composite);
    }
```

| 参数名      | 介绍                                        |
| ----------- | ------------------------------------------- |
| *composite* | **RookiesGoods_Composite** 合成物品类的对象 |



### 2.合成失败回执

> OnComposeFailed(RookiesGoods_Composite composite)

**功能说明：**

```
合成失败时会调用这个函数，一般来说，产生失败是因为没有足够的材料，可以借此通知玩家。
```

| 参数名      | 介绍                                        |
| ----------- | ------------------------------------------- |
| *composite* | **RookiesGoods_Composite** 合成物品类的对象 |





### 3.装备成功回执

> OnSuitUpSuccessful(RookiesGoods_SuitBase suit)

**功能说明：**

> 装备某装备物品成功时会调用这个函数，可以在这个函数中对角色的属性进行比如增益的操作。

**例子：**

```C#
    public override void OnSuitUpSuccessful(RookiesGoods_SuitBase suit)
    {
        //尝试获取属性名为defence(防御力)的属性
        object defence = suit.TryGetProperty("defence");
        if (defence != null)
            //拥有这个属性，将其输出，开发者可以将值传递给
            Debug.Log("装备了防御力为:" + (int)defence + "的装备" + suit.Name);
    }
```

 *注：例子中只是输出属性值，实际情况上应向该角色的人物属性脚本调用调整属性的函数。在此不做过多演示。*

| 参数名 | 介绍                                   |
| ------ | -------------------------------------- |
| suit   | **RookiesGoods_SuitBase** 装备类的对象 |



### 4.卸装成功回执

> OnTakeOffSuccessful(RookiesGoods_SuitBase suit ,bool isOver)

**功能说明：**

```
卸下装备成功是会调用这个函数，可以在这个函数中对角色的属性进行比如减益的操作。
```

**例子：**

```C#
    public override void OnTakeOffSuccessful(RookiesGoods_SuitBase suit,bool isOver)
    {
        //尝试获取属性名为defence(防御力)的属性
        object defence = suit.TryGetProperty("defence");
        if (defence != null)
        {
            //是否是因为耐久度归零而导致的卸装
            if (isOver)
                 Debug.Log(suit.Name+"损坏了");
           Debug.Log("卸下了防御力为:" + (int)defence + "的装备" + suit.Name);
        }
    }
```

 *注：例子中只是输出属性值，实际情况上应向该角色的人物属性脚本调用调整属性的函数。在此不做过多演示。*

| 参数名 | 介绍                                             |
| ------ | ------------------------------------------------ |
| suit   | **RookiesGoods_SuitBase** 装备类的对象           |
| isOver | **bool** 布尔值 是否是因为耐久度归零而导致的卸装 |

**返回：**  

无返回值。



### 5.使用物品回执

> OnUsedSomething(RookiesGoods_Consumable consumable)

**功能说明：**

> 使用某物品时会调用这个函数，可以查看这个物品的自定义属性，对角色进行一些增益、减益的操作。

**例子：**

```C#
    public override void OnUsedSomething(RookiesGoods_Consumable consumable)
    {
        //尝试获取属性名为hp(恢复生命值)的属性
        object hp = consumable.TryGetProperty("hp");
        if (hp != null)
            Debug.Log("恢复了" + (int)hp + "的生命值");
    }
```

 *注：例子中只是输出属性值，实际情况上应向该角色的人物属性脚本调用调整属性的函数。在此不做过多演示。*

| 参数名     | 介绍                                      |
| ---------- | ----------------------------------------- |
| consumable | **RookiesGoods_Consumable**消耗品类的对象 |



# 相关API

[RookiesGood's APIList](https://github.com/ConfuseL/RookiesGoods/wiki)
