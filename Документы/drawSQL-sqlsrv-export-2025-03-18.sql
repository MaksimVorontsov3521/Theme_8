CREATE TABLE "Client"(
    "ClientID" INT NOT NULL IDENTITY(1,1),
    "ClientName" NVARCHAR(255) NOT NULL,
    "INN" VARCHAR(10) NULL,
    "Email" NVARCHAR(255) NULL,
    "OGRN" VARCHAR(13) NULL,
    "KPP" VARCHAR(10) NULL
);
ALTER TABLE
    "Client" ADD CONSTRAINT "client_clientid_primary" PRIMARY KEY("ClientID");
CREATE TABLE "UserTable"(
    "UserID" INT NOT NULL IDENTITY(1,1),
    "UserName" NVARCHAR(255) NOT NULL,
    "Surname" NVARCHAR(255) NULL,
    "Patronymic" NVARCHAR(255) NULL,
    "PhoneNumber" VARCHAR(255) NULL,
    "RoleID" INT NOT NULL,
    "DepartmentID" INT NOT NULL,
    "UserLogin" NVARCHAR(255) NOT NULL,
    "UserPassword" NVARCHAR(255) NOT NULL
);
ALTER TABLE
    "UserTable" ADD CONSTRAINT "usertable_userid_primary" PRIMARY KEY("UserID");
CREATE TABLE "RoleTable"(
    "RoleID" INT NOT NULL IDENTITY(1,1),
    "RoleName" NVARCHAR(255) NOT NULL,
    "Rights" NVARCHAR(255) NULL,
    "Level" INT NOT NULL
);
ALTER TABLE
    "RoleTable" ADD CONSTRAINT "roletable_roleid_primary" PRIMARY KEY("RoleID");
CREATE TABLE "Document"(
    "DocumentID" INT NOT NULL IDENTITY(1,1),
    "FolderID" INT NOT NULL,
    "Name" NVARCHAR(255) NOT NULL,
    "IsDone" BIT NOT NULL,
    "DeadLine" DATE NOT NULL,
    "ReadOnly" BIT NOT NULL,
    "NameInPattertn" NVARCHAR(255) NULL
);
ALTER TABLE
    "Document" ADD CONSTRAINT "document_documentid_primary" PRIMARY KEY("DocumentID");
CREATE TABLE "Folder"(
    "FolderID" INT NOT NULL IDENTITY(1,1),
    "FolderPath" NVARCHAR(255) NOT NULL,
    "PatternID" INT NULL,
    "ClientID" INT NOT NULL
);
ALTER TABLE
    "Folder" ADD CONSTRAINT "folder_folderid_primary" PRIMARY KEY("FolderID");
CREATE TABLE "Log"(
    "LogID" INT NOT NULL IDENTITY(1,1),
    "UserID" INT NOT NULL,
    "DocumentID" INT NULL,
    "LogAction" INT NOT NULL
);
ALTER TABLE
    "Log" ADD CONSTRAINT "log_logid_primary" PRIMARY KEY("LogID");
CREATE TABLE "LogAction"(
    "ActionID" INT NOT NULL IDENTITY(1,1),
    "ActionName" NVARCHAR(255) NOT NULL
);
ALTER TABLE
    "LogAction" ADD CONSTRAINT "logaction_actionid_primary" PRIMARY KEY("ActionID");
CREATE TABLE "Department_Folder"(
    "Department_Folder_ID" INT NOT NULL IDENTITY(1,1),
    "DepartmentID" INT NOT NULL,
    "FolderID" INT NOT NULL
);
ALTER TABLE
    "Department_Folder" ADD CONSTRAINT "department_folder_department_folder_id_primary" PRIMARY KEY("Department_Folder_ID");
CREATE TABLE "Department"(
    "DepartmentID" INT NOT NULL IDENTITY(1,1),
    "DepartmentName" NVARCHAR(255) NOT NULL
);
ALTER TABLE
    "Department" ADD CONSTRAINT "department_departmentid_primary" PRIMARY KEY("DepartmentID");
CREATE TABLE "Required_in_the_pattern"(
    "id" INT NOT NULL IDENTITY(1,1),
    "PatternID" INT NOT NULL,
    "Name" NVARCHAR(255) NOT NULL
);
ALTER TABLE
    "Required_in_the_pattern" ADD CONSTRAINT "required_in_the_pattern_id_primary" PRIMARY KEY("id");
CREATE TABLE "Pattern"(
    "PatternID" INT NOT NULL IDENTITY(1,1),
    "PatternName" NVARCHAR(255) NOT NULL,
    "Description" NVARCHAR(255) NULL
);
ALTER TABLE
    "Pattern" ADD CONSTRAINT "pattern_patternid_primary" PRIMARY KEY("PatternID");
ALTER TABLE
    "Log" ADD CONSTRAINT "log_documentid_foreign" FOREIGN KEY("DocumentID") REFERENCES "Document"("DocumentID");
ALTER TABLE
    "Document" ADD CONSTRAINT "document_folderid_foreign" FOREIGN KEY("FolderID") REFERENCES "Folder"("FolderID");
ALTER TABLE
    "Log" ADD CONSTRAINT "log_logaction_foreign" FOREIGN KEY("LogAction") REFERENCES "LogAction"("ActionID");
ALTER TABLE
    "Log" ADD CONSTRAINT "log_userid_foreign" FOREIGN KEY("UserID") REFERENCES "UserTable"("UserID");
ALTER TABLE
    "Folder" ADD CONSTRAINT "folder_clientid_foreign" FOREIGN KEY("ClientID") REFERENCES "Client"("ClientID");
ALTER TABLE
    "Folder" ADD CONSTRAINT "folder_patternid_foreign" FOREIGN KEY("PatternID") REFERENCES "Pattern"("PatternID");
ALTER TABLE
    "Department_Folder" ADD CONSTRAINT "department_folder_departmentid_foreign" FOREIGN KEY("DepartmentID") REFERENCES "Department"("DepartmentID");
ALTER TABLE
    "UserTable" ADD CONSTRAINT "usertable_departmentid_foreign" FOREIGN KEY("DepartmentID") REFERENCES "Department"("DepartmentID");
ALTER TABLE
    "Required_in_the_pattern" ADD CONSTRAINT "required_in_the_pattern_patternid_foreign" FOREIGN KEY("PatternID") REFERENCES "Pattern"("PatternID");
ALTER TABLE
    "Department_Folder" ADD CONSTRAINT "department_folder_folderid_foreign" FOREIGN KEY("FolderID") REFERENCES "Folder"("FolderID");
ALTER TABLE
    "UserTable" ADD CONSTRAINT "usertable_roleid_foreign" FOREIGN KEY("RoleID") REFERENCES "RoleTable"("RoleID");