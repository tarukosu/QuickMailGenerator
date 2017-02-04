![](demo.gif)

# ダウンロード
[Releaseページ](https://github.com/tarukosu/QuickMailGenerator/releases) より、zipファイルをダウンロードして展開  
QuickMailGenerator.exe を実行

# 設定ファイル
config/Settting.json で設定

### menu
テンプレートを開くボタンの設定
```
{
  "menu": [
    {
      "id": "template1",
      "name": "テンプレート1"
    },
    ...
  ],
  ...
}
```

- id: ボタンを押した際に開くテンプレートID
- name: ボタンに表示する文字列

### general
全体設定
```
{
  "general": {
    "input": [
      {
        "name": "user",
        "description": "user name",
        "default": "default user name"
      }
      ...
    ]
  },
```
- input:  テンプレートで共通して使う入力ボックス設定

### templates
メールテンプレートの定義
```
  {
   "templates": [
    {
      "id": "template1",
      "name": "テンプレート1",
      "to": "to@sample.com",
      "cc": "cc@sample.com",
      "bcc": "bcc@sample.com",
      "subject": "メールタイトル",
      "body": [ "メール本文 {user}" ]
      "input": [
        {
          "name": "user",
          "default": "user name"
        },
        ...
      ]
    },
    ...
   ]
  }
```

- id: テンプレートID　(menuのIDと紐づける)
- name: テンプレート名
- to: メールの To 欄に入力するアドレス
- cc: メールの Cc 欄に入力するアドレス
- bcc: メールの Bcc 欄に入力するアドレス
- subject: メール件名
- body: メール本文
- input: 入力用テキストボックス設定

to, cc, bcc, subject, body にはテキストボックスで入力する文字列を埋め込むことができる。  
input で設定した項目の name の文字列を { } で囲んで文字列内に埋め込む。  
例えば, `"name": "user"` として設定した項目のテキストを使う場合は`{user}`を埋め込んでおく。

### input
入力テキストボックスの設定
```
"input": [
    {
      "name": "header",
      "description": "ヘッダ",
      "type": "Multiline",
      "default": "こんにちは"
    },
    ...
]
```
- name: 文字列を埋め込む際に使う名前
- description: テキストボックスの詳細説明
- type: 入力タイプ (設定なしの場合 Singleline)
    - Singleline: 一行
    - Multiline:  複数行
- default: 初期値

# License
MIT
