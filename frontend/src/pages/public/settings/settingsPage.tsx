import { useState, useEffect } from 'react';
import { UserService } from '../../../services/UserService';

const API_BASE = "http://localhost:5000";

interface User {
  id: number;
  username: string;
  profileImage?: string;
}

export default function Settings() {
  const [user, setUser] = useState<User | null>(null);
  const [imageFile, setImageFile] = useState<File | null>(null);
  const [preview, setPreview] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    loadUser();
  }, []);

  const loadUser = async () => {
    try {
      const data = await UserService.getCurrentUser();
      setUser(data);
    } catch (err) {
      console.error('Failed to load user:', err);
    }
  };

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (file) {
      setImageFile(file);
      setPreview(URL.createObjectURL(file));
    }
  };

  const handleUpload = async () => {
    if (!imageFile || !user) return;
    
    setLoading(true);
    try {
      const updatedUser = await UserService.updateProfileImage(user.id, imageFile);
      setUser(updatedUser); // Atnaujinam state su nauja nuotrauka
      setImageFile(null);
      setPreview(null); // Išvalom preview
      alert('Profile image updated!');
    } catch (err) {
      alert('Failed to update profile image');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const profileSrc = preview || (user?.profileImage 
    ? `${API_BASE}${user.profileImage}`
    : `https://api.dicebear.com/7.x/initials/svg?seed=${user?.username || 'User'}`);

  if (!user) return <div className="p-6 text-white">Loading...</div>;

  return (
    <div className="p-6 max-w-2xl mx-auto">
      <h1 className="text-3xl font-bold text-white mb-6">Settings</h1>
      
      <div className="bg-neutral-900 rounded-2xl p-6">
        <h2 className="text-xl font-semibold text-white mb-4">Profile Picture</h2>
        
        <div className="flex items-center gap-6">
          <img
            src={profileSrc}
            alt="Profile"
            className="w-32 h-32 rounded-full object-cover border-4 border-green-500" // i neutral jei ka 
          />
          
          <div className="flex flex-col gap-2">
            <input
              type="file"
              accept="image/*"
              onChange={handleFileChange}
              className="text-gray-300 text-sm"
            />
            {imageFile && (
              <button
                onClick={handleUpload}
                disabled={loading}
                className="bg-green-500 hover:bg-green-600 disabled:opacity-50 text-white px-4 py-2 rounded-lg transition-colors"
              >
                {loading ? 'Uploading...' : 'Upload'}
              </button>
            )}
          </div>
        </div>
        
        <div className="mt-6 space-y-2 text-gray-300">
          <p><strong>Username:</strong> {user.username}</p>
        </div>
      </div>
    </div>
  );
}