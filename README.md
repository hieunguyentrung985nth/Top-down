# Clone Alien Shooter 2D

Đây là game clone dựa trên game gốc Alien Shooter của Sigma.

## Cài đặt

Dùng Git lấy code về, sau đó mở Unity chọn project. Sau khi Unity cài đặt các package và mở project lên thành công, vào thư mục Scenes chọn SampleScene, chỉnh độ phân giải cho phù hợp, cuối cùng là Run game.

## Tính năng game

1.	Thay đổi âm lượng cho nhạc nền và nhạc hiệu ứng
 
2.	2 chế độ: Campaign và Survivai. Campaign sẽ là chế độ đi theo màn (tổng 3 màn), Survival sẽ là chế độ sinh tồn lâu nhất có thể với độ khó tăng dần theo thời gian. Có 2 nhân vật để lựa chọn: Nam (trái) và Nữ (phải) với mỗi nhân vật có chỉ số khác nhau. (Máu – Tốc độ di chuyển – Lượng đạn tối đa lưu trữ – Độ chính xác) 
 
3.	Khi tham gia Campaign sẽ vào phần mua đồ. Người chơi dùng tiền để mua đồ. Đồ bao gồm 1 vật phẩm đặc biệt để tăng máu tối đa và 6 súng. Các súng trong này lần lượt tương ứng với súng trong game gốc:
•	Từ súng 1 – 4 tương đương súng 1 – 4 trong game gốc
•	Súng 5 tương đương với súng số 7 trong game gốc
•	Súng 6 tương đương với súng số 9 trong game gốc
Hiển thị thông tin từng vật phẩm khi di chuột vào, thông báo không đủ tiền nếu không đủ tiền để mua, không mua được nữa nếu đã có hoặc tối đa lượng có thể chứa. Ngoài ra có nút Hack Money để hack tiền.
 
4.	Về gameplay sẽ có các quái vật. Trong game sẽ có 3 loại quái tương đương với game gốc:
•	Tam giác – quái nhỏ 1 trong game gốc
•	Capsule – quái lớn 2 trong game gốc (con mà chạy 2 chân và cào bằng 2 tay)
•	Capsule súng – giống quái 2 nhưng có súng
Cấp độ mỗi con cũng sẽ là từ xanh lá -> vàng -> đỏ -> xanh biển. Với con có súng sau khi bắn phá bộ súng, chúng sẽ biến thành con không có súng.
Trong Campaign, người chơi bắn hạ tất cả các quái để qua màn, bắn phá các thùng để nhặt vật phẩm. Khi trên bản đồ còn 1 con quái, hệ thống sẽ hiện mũi tên chỉ hướng tới con quái đó.
Trong Survival cách chơi tương tự, người chơi lần lượt qua các Phase, hạ gục con xanh nước biển để nhặt súng mới và tiếp tục sinh tồn. (trong chế độ này sẽ không rơi ra tiền). Sau khi kết thúc sẽ đánh giá thành quả người chơi đạt được.
5.	Save game sẽ lưu thành file và lưu ở C:\User\{user}\AppData…
6.	Âm thanh đi bộ, nút bấm, bắn súng,…


## Hạn chế

1. Ngoài các tính năng kể trên thì còn nhiều hạn chế của game: ít quái vật, ít vật phẩm, ít màn, đồ họa, giao diện xấu, tối ưu kém, ....
2. Do là project đầu tay nên code viết chưa được tốt, tối ưu.

## Link Google Drive

Link bao gồm mô tả + hình ảnh + video + file build: https://drive.google.com/drive/folders/1v5kpwY_RBF61tjCbNmXbQ47ZNjEUvkN6
