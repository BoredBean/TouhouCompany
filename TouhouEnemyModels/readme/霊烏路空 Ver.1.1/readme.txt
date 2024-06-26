MikuMikuDance用モデルデータ　霊烏路空 Ver.1.1

ダウンロード頂きありがとうございます。
このデータは「霊烏路空」の「MikuMikuDance」用モデルデータです。


◆注意事項

・「霊烏路空」(以下「お空」）は上海アリス幻樂団様の「東方Project」に登場する
　キャラクターです。そのため本モデルは上海アリス幻樂団様のキャラクター利用の
　ガイドラインに沿って使用下さるようお願いします。
　（ガイドラインは上海アリス幻樂団様のHPに書かれています。）

・本モデルはPMX形式のモデルのため、動作にはMMD ver.7.31以降が必要です。また
　一部のギミックはMMEを使用することを前提に作成しております。

・このモデルを使用した作品を動画投稿サイト等に投稿する場合、当方への連絡など
　は特に必要ありません。原則として自由にご使用ください。ただし、著しく公序良
　俗に反するような動画への使用はご遠慮ください。

・データの商用利用及び二次配布は原則禁止です。データの改造は自由ですが、改造
　したデータを無断で再配布することは禁止します。

・本モデルを使用して何か問題が発生したとしても、当方は責任を負えません。使用
　する際は、自己責任でお願いします。




◆モデルのギミックについての解説

お空モデルには他のモデルには無い特殊なギミックが複数組み込まれています。
以下はその解説です。
（解説中には宇宙マントのように正式な名称ではないけれど、説明の便宜上使用して
いる用語がありますのでご注意ください。）



○黒い太陽

黒い太陽とはお空の頭上にある光る黒い物体のことです。本モデルでは、そぼろ様が
製作されたMMEエフェクト「AutoLuminous」のマスク機能を使用することでこれを再現
しています。

手順
・まずは「AutoLuminous」を用意してください。（ニコニコ動画にそぼろ様の公開動画
が投稿されていますので、すぐに見つけることが出来ると思います。）

・「AutoLuminous」に付属されている「Readme上級編.txt」の記載にしたがってマスク
　機能を使えるようにしてください。

・MMDにお空モデルと「AutoLuminous」を読込みます。

・お空モデルの黒い太陽をマスクします。具体的な方法は次の通りです。

　MMD右上の「MMEffect」→「エフェクト割当」→「AL_MaskRT」→お空モデルを右クリック
　→サブセット展開→「subset#」の一番下の項目（例えばマント有、制御棒有モデルなら
　「subset #44」です。）をクリックして解除を選択

・お空モデルの表情操作「その他」にある「黒い太陽」を操作して「黒い太陽」を表示
　させます。



○左足（分解の足）の光球

お空の左足にある光球にはMMDエンジンが組み込まれています。左足のすねのところに
ある「光球回転」ボーンを少し動かすと光球が回りだします。

もしうまく回らない場合は「光球回転」ボーンを選択後、「ボーン編集」の「数値入力」
を開き、そこの「角度」の「Y軸」の欄に直接数字を入力してみてください。-1ぐらいが
お勧めです。

回転角度は「光球X角度」で調整してください。「光球X拡大」で回転の大きさを、「光球X短」
で軌跡の長さを調整できます。

この光球も「AutoLuminous」に対応しています。





○宇宙マント

宇宙が映し出されているお空のマントのことです。
本モデルのマントには、マントを通して見ると他のモデルやアクセサリが透けて見える
ようになる機能（透過機能）が付けられています。いわゆる「紳士枠」と呼ばれる方法
です。

この機能を使用するためにはモデルやアクセサリの描画の順番が大切になります。

MMDにはそれそれのモデルやアクセサリを画面に描画する順番が設定されています。
この順番にはルールがあり、後に描画されたものは先に描画されたものを透過することが
出来ません。つまり、宇宙を１番目、お空を２番目、他のモデルを３番目以降の順番で
描画させることによって宇宙は透過せずに他のモデルやアクセサリだけを透過させ、
マントに宇宙画像を映しだすことができるようになります。

手順

・最初にお空モデル、その他必要なモデル及びお空モデルに同梱している「宇宙.pmx」
　をMMDに読込んでください。

・次にお空モデルの表情操作の「その他」にある「宇宙ﾏﾝﾄ」のスライダーを一番右まで
　動かしてください。お空のマントが透過し、宇宙が映し出されたと思います。

・もし他のモデルがマントに写ってしまっている場合、モデルの描画順をチェックして
　みてください。MMD上部の「背景」にある「モデル描画順」を使って上から宇宙、お空、
　その他のモデルの順になるよう入れ替えてみてください。

・アクセサリについてはモデルと異なり、描画順の変更にはMMD上部の「背景」にある
　「アクセサリ編集」を使用します。アクセサリもモデルと同様に描画順をお空よりも
　後になるよう設定してください。

以上の方法で宇宙マントができるはずです。

その他

・「宇宙.pmx」には、いわゆるMMDエンジンが組み込まれており、自動で回転させることが
　出来ます。「回転ボーン」を少し捻ると回りだします。フレーム登録を忘れると止まって
　しまいますので注意してください。回転角度の調整は「宇宙.pmx」の「センターボーン」
　を使用してください。

・「宇宙.pmx」は、そぼろ様製作のMM発光Eエフェクト「AutoLuminous」に対応しています。
　使用すると星が明るくなります。






本モデルに組み込まれているPMX用MMDエンジンはFuria様が作成されたものを使用させて
いただきました。

髪、翼、ヤタの目に使用させていただいてるスフィアマップはスフィアマッＰ様製作の
「テカテカスフィアマップ」及び「hair2-b.bmp」を加工して使用させていただきました。

その他本モデルの作成に当たっては多くの方のアイデアや意見を参考にさせていただきま
した。

本当にありがとうございます。

もし本モデルを使用中におかしな点、改善すべき点など何か気づかれたことがありましたら
動画へのコメントやメールなどで教えていただけると大変助かります。

Ver.1.1完成（2012/2/1）
「霊烏路空　（マント無　制御棒無）」モデルのボーンの修正
ヤタの目に発光ギミックを追加

Ver.1.0完成（2012/2/1）

モデル制作　zakoneko
連絡先　zakoneko@yahoo.co.jp
