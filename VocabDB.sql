CREATE TABLE Users (
    user_id INT PRIMARY KEY IDENTITY(1,1),
    username NVARCHAR(50) NOT NULL,
    password NVARCHAR(255) NOT NULL,
    email NVARCHAR(100) NOT NULL,
    level INT NOT NULL,
    subscription NVARCHAR(50),
    subscription_period DATE,
    notification_type NVARCHAR(50),
    reset_password_link NVARCHAR(255),
    role NVARCHAR(50) NOT NULL,
    notification_time TIME
);

CREATE TABLE Category (
    category_id INT PRIMARY KEY IDENTITY(1,1),
    category_name NVARCHAR(255) NOT NULL,
    user_id INT,
    FOREIGN KEY (user_id) REFERENCES Users(user_id) ON DELETE CASCADE
);

CREATE TABLE Word (
    word_id INT PRIMARY KEY IDENTITY(1,1),
    name NVARCHAR(100) NOT NULL,
    translation NVARCHAR(255) NOT NULL,
    category_id INT,
    img_link NVARCHAR(255),
    repetition_num INT NOT NULL,
    FOREIGN KEY (category_id) REFERENCES Category(category_id) ON DELETE CASCADE
);

CREATE TABLE Repetition (
    repetition_id INT PRIMARY KEY IDENTITY(1,1),
    repetition_date DATE NOT NULL,
    word_id INT,
    FOREIGN KEY (word_id) REFERENCES Word(word_id) ON DELETE CASCADE
);

CREATE TABLE Message (
    message_id INT PRIMARY KEY IDENTITY(1,1),
    message NVARCHAR(MAX) NOT NULL,
    user_id INT,
    admin_id INT,
    is_shown BIT NOT NULL,
    FOREIGN KEY (user_id) REFERENCES Users(user_id) ON DELETE CASCADE,
    FOREIGN KEY (admin_id) REFERENCES Users(user_id) ON DELETE NO ACTION
);
