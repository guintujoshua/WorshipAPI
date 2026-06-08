# Password Hashing Implementation

## Overview
Secure password hashing using BCrypt has been implemented for all user operations.

## Important: Hashing vs Encryption

### ❌ Why NOT Encryption?
- **Passwords should NEVER be decrypted**
- If someone gets the encryption key, all passwords are exposed
- Encryption is reversible (not secure for passwords)

### ✅ Why Hashing (BCrypt)?
- **One-way function** - Cannot be reversed
- **Salted** - Each password has unique salt
- **Work factor** - Configurable computational cost (set to 12)
- **Industry standard** - Used by major platforms
- **Future-proof** - Designed to be slow to resist brute force

## How It Works

### Creating/Updating Users:
1. Client sends **plain password** in JSON
2. Service layer **hashes** the password using BCrypt
3. **Only the hash** is stored in database
4. Original password is never stored

### Verifying Passwords (for login):
1. Client sends plain password
2. Service retrieves hashed password from database
3. BCrypt verifies plain password against hash
4. Returns true/false (no decryption needed)

## Implementation Details

### Components Created:

#### 1. **IPasswordHasher Interface** (`Services/Security/`)
```csharp
public interface IPasswordHasher
{
	string HashPassword(string password);
	bool VerifyPassword(string password, string hashedPassword);
}
```

#### 2. **PasswordHasher Implementation**
- Uses BCrypt with work factor of 12
- Automatically handles salting
- Thread-safe and stateless

#### 3. **User DTOs** (`Models/Users/UserDtos.cs`)
- **CreateUserDto** - For creating users (includes plain password)
- **UpdateUserDto** - For updating users (optional password change)
- **UserDto** - For responses (NO password field)

### API Flow:

#### Create User:
```json
POST /api/User
{
	"name": "John Doe",
	"email": "john@example.com",
	"role": "Admin",
	"password": "MySecurePassword123!",  // Plain text
	"status": "Active"
}
```
↓ Hashed by service layer ↓
```
$2a$12$N0W9qJlmQxjIZjKvdj2eGO9AJl5P2TZvdKm9jHn3PqFjKd...
```
↓ Stored in database ↓

#### Response (Password excluded):
```json
{
	"id": 1,
	"name": "John Doe",
	"email": "john@example.com",
	"role": "Admin",
	"status": "Active"
	// NO password field in response!
}
```

#### Update User:
```json
PUT /api/User/1
{
	"name": "John Doe",
	"email": "john@example.com",
	"role": "Admin",
	"password": "NewPassword456!"  // Optional - only if changing password
}
```

## Security Benefits

### ✅ Implemented:
- **BCrypt hashing** with strong work factor
- **Automatic salting** (unique per password)
- **No password exposure** in API responses
- **One-way hashing** (cannot be reversed)
- **Separation of concerns** (hashing in service layer)

### 🔒 Best Practices Followed:
- Passwords never stored in plain text
- Passwords never returned in API responses
- Hashing happens in service layer (business logic)
- Using industry-standard BCrypt algorithm
- Configurable work factor for future-proofing

## Testing

### Create a User:
```bash
POST /api/User
{
	"name": "Test User",
	"email": "test@example.com",
	"role": "User",
	"password": "TestPassword123",
	"status": "Active"
}
```

### Check Database:
```sql
SELECT Id, Name, Email, HashedPassword FROM Users WHERE Email = 'test@example.com';
```
You'll see something like:
```
HashedPassword: $2a$12$N0W9qJlmQxjIZjKvdj2eGO9AJl5P2TZvdKm9jHn3PqFjKd...
```

### Verify GET Response:
```bash
GET /api/User/1
```
Response will NOT include password:
```json
{
	"id": 1,
	"name": "Test User",
	"email": "test@example.com",
	"role": "User",
	"status": "Active"
}
```

## Future Enhancements

Consider adding:
1. **Login endpoint** - Use `VerifyPassword` method
2. **JWT tokens** - For authentication
3. **Password strength validation** - Minimum requirements
4. **Email verification** - Before account activation
5. **Password reset** - Via email token

## Dependencies
- **BCrypt.Net-Next** (v4.2.0) - Password hashing library
- Registered as Singleton in DI container
- Zero configuration needed
