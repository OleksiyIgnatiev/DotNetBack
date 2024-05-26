CREATE TABLE Users (
    user_id INT PRIMARY KEY IDENTITY(1,1),
    username VARCHAR(50) NOT NULL,
    password VARCHAR(255) NOT NULL,
    email VARCHAR(100) NOT NULL,
    level INT NOT NULL,
    subscription VARCHAR(50) NOT NULL,
    subscription_period DATE NOT NULL,
    notification_type VARCHAR(50) NOT NULL,
    reset_password_link VARCHAR(255),
    role VARCHAR(50) NOT NULL,
    notification_time TIME NOT NULL
);

CREATE TABLE Category (
    category_id INT PRIMARY KEY IDENTITY(1,1),
    category_name VARCHAR(255) NOT NULL,
    user_id INT,
    FOREIGN KEY (user_id) REFERENCES Users(user_id) ON DELETE CASCADE
);


CREATE TABLE Word (
    word_id INT PRIMARY KEY IDENTITY(1,1),
    name VARCHAR(100) NOT NULL,
    translation VARCHAR(255) NOT NULL,
    category_id INT,
    img_link VARCHAR(255),
    repetition_num INT NOT NULL,
    repetition_date DATE NOT NULL,
    FOREIGN KEY (category_id) REFERENCES Category(category_id) ON DELETE CASCADE
);

CREATE TABLE Message (
    message_id INT PRIMARY KEY IDENTITY(1,1),
    message TEXT NOT NULL,
    user_id INT,
    admin_id INT,
    is_shown BIT NOT NULL,
    FOREIGN KEY (user_id) REFERENCES Users(user_id),
    FOREIGN KEY (admin_id) REFERENCES Users(user_id)
);
