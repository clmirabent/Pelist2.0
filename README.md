# 🎬 Pelist

**Pelist** is my final project for the C# and .NET Bootcamp by Fundación Esplai.  
It's a full-stack web application focused on social interaction, where users can share opinions and connect with others through their favorite movies.

---

## 📌 Project Description

Pelist is designed for people who don’t know what movie to watch next.  
Users can browse a catalog of films reviewed by other users, read comments, and interact with the community. Each user can:

- Leave reviews and ratings
- Add movies to personal lists (Completed, Abandoned, Pending, Favorites)
- Manage their own profile and friendships
- Share movie preferences and explore others’ profiles

---

## 🚀 Main Features

### 🔐 Authentication

- Users can register and login securely.
- The backend validates if the email already exists.
- Bio character limit: 120 characters with a real-time countdown.
- Profile pictures are uploaded using **Cloudinary**.

<img width="812" alt="Screenshot 2025-06-03 at 09 48 08" src="https://github.com/user-attachments/assets/bfded079-1072-463c-bc1e-93255375bd88" />

### 🎥 Movie Details

- Once logged in, users can:
  - Add movies to different personal lists (Completed, Abandoned, Pending, Favorites)
  - Leave a public comment on any movie

<img width="817" alt="Screenshot 2025-06-03 at 09 48 18" src="https://github.com/user-attachments/assets/63ed8c04-e8c1-4591-bcef-4606a36c8e98" />

<img width="730" alt="Screenshot 2025-06-03 at 09 48 24" src="https://github.com/user-attachments/assets/3278b51f-2e15-468b-b71b-6f8d3a0efcdb" />



### 👤 User Profile

- Users can:
  - View their own lists, friends, and comments
  - Manage incoming friend requests (accept/reject)
<img width="810" alt="Screenshot 2025-06-03 at 09 48 33" src="https://github.com/user-attachments/assets/2371f42c-db5b-468a-88e7-3164bc0f6760" />

<img width="816" alt="Screenshot 2025-06-03 at 09 48 47" src="https://github.com/user-attachments/assets/e5534c56-5cf5-4d95-94a6-5f2f2ad00b06" />


### 🔍 Friend Search

- Users can:
  - Search for other users by username
  - Send friend requests (button disabled if already sent)
  - Receive alerts if the user doesn’t exist

<img width="817" alt="Screenshot 2025-06-03 at 09 48 40" src="https://github.com/user-attachments/assets/b9a3c198-398b-4676-8622-c2e1b0505651" />



### 🤝 Friend System

- You can only access another user’s profile if you're friends.
- If you're not connected, a message will inform you that the profile is private.


## 🛠️ Technologies Used

- **Frontend:** ASP.NET MVC
- **Database:** SQL Server + Entity Framework
- **Movie API:** [TMDB](https://www.themoviedb.org/)
- **Image Hosting:** [Cloudinary](https://cloudinary.com/)
- **Notifications:** SweetAlert

---

## 📚 What I Learned

- How to configure **Entity Framework** and connect to a relational database
- Created the full **User Profile system**, including:
  - Friend request logic
  - Personal lists
  - UX-focused search flow
- Understood the importance of **planning database relationships** early in development  
- Developed a deeper sense of empathy for the user’s experience and clarity in microcopy (UX writing)
- 💡 *Learned how to build scalable features by breaking down complex relationships between users, data, and permissions (e.g., only friends can see profiles).*

---

## ⚙️ Installation & Status

> ⚠️ *Currently migrating the SQL Server database to Supabase for easier deployment.*

Once finished, deployment instructions will be added here.

---

Pelist is not just a movie web app, it's a reflection of what I learned and loved during my time in the bootcamp.  
It combines code, creativity, and community, just like the kind of developer I aim to be.

---

### 📬 Contact

Find me on [LinkedIn](https://www.linkedin.com/in/carlaleonmirabent/)


