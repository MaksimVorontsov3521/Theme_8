CREATE TABLE Client(
    ClientID INT PRIMARY KEY IDENTITY (1,1),
    ClientName NVARCHAR(255) NOT NULL,
    INN VARCHAR(10),
    Email NVARCHAR(255),
    OGRN VARCHAR(13),
    KPP VARCHAR(10) 
);
CREATE TABLE RoleTable(
    RoleID INT PRIMARY KEY IDENTITY (1,1),
    RoleName NVARCHAR(255) NOT NULL,
    Rights NVARCHAR(255),
    RoleLevel INT NOT NULL
);
CREATE TABLE LogAction(
    ActionID INT PRIMARY KEY IDENTITY (1,1),
    ActionName NVARCHAR(255) NOT NULL
);
CREATE TABLE Pattern(
    PatternID INT PRIMARY KEY IDENTITY (1,1),
    PatternName NVARCHAR(255) NOT NULL UNIQUE,
    PatternDescription NVARCHAR(255)
);
CREATE TABLE Department(
    DepartmentID INT PRIMARY KEY IDENTITY (1,1),
    DepartmentName NVARCHAR(255) NOT NULL UNIQUE
);

CREATE TABLE UserTable(
    UserID INT PRIMARY KEY IDENTITY (1,1),
    UserName NVARCHAR(255) NOT NULL,
    Surname NVARCHAR(255),
    Patronymic NVARCHAR(255),
    PhoneNumber VARCHAR(255),
    RoleID INT NOT NULL REFERENCES  RoleTable(RoleID),
    DepartmentID INT NOT NULL REFERENCES Department(DepartmentID),
    UserLogin NVARCHAR(255) NOT NULL UNIQUE,
    UserPassword NVARCHAR(255) NOT NULL,
    Blocked BIT NOT NULL DEFAULT 'false',
    Email NVARCHAR(255)
);

CREATE TABLE Folder(
    FolderID INT PRIMARY KEY IDENTITY (1,1),
    FolderPath NVARCHAR(255) NOT NULL UNIQUE,
    PatternID INT  REFERENCES Pattern(PatternID),
    DeadLine DATETIME2,
    IsDone BIT DEFAULT false,
    ClientID INT REFERENCES Client(ClientID)
);
CREATE TABLE RequiredInPattern(
    DocumentPatternID INT PRIMARY KEY IDENTITY (1,1),
    PatternID INT NOT NULL REFERENCES Pattern(PatternID),
    DocumentName NVARCHAR(255) NOT NULL
);
CREATE TABLE Document(
    DocumentID INT PRIMARY KEY IDENTITY (1,1),
    FolderID INT NOT NULL REFERENCES Folder(FolderID),
    DocumentName NVARCHAR(255) NOT NULL,   
    DocumentReadOnly BIT NOT NULL DEFAULT 'false',
    InPatternID INT REFERENCES RequiredInPattern(PatternID)
);

CREATE TABLE LogTable(
    LogID INT PRIMARY KEY IDENTITY (1,1),
    UserID INT NOT NULL REFERENCES UserTable(UserID),
    DocumentID INT REFERENCES Document(DocumentID),
    LogAction INT NOT NULL REFERENCES LogAction(ActionID),
	LogDate DATETIME2 
);

CREATE TABLE DepartmentFolder(
    DepartmentFolderID INT PRIMARY KEY IDENTITY (1,1),
    DepartmentID INT NOT NULL REFERENCES Department(DepartmentID),
    FolderID INT NOT NULL REFERENCES Folder(FolderID)
);
