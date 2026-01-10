"use client"

import Link from "next/link"
import { useAuth } from "@/contexts/AuthContext"
import { updateProfile, changePassword } from "@/services/authService"
import { useEffect, useState } from "react"
import { CheckCircle2, Trash } from "lucide-react"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import {
    Card,
    CardContent,
    CardDescription,
    CardFooter,
    CardHeader,
    CardTitle,
} from "@/components/ui/card"
import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar"

export default function EditProfilePage() {
    const { user, refreshAuth } = useAuth()
    const [isSaved, setIsSaved] = useState(false)
    const [formData, setFormData] = useState({
        name: "",
        username: "",
        email: "",
        avatar: "/images/avatar.png"
    })

    useEffect(() => {
        if (user) {
            
            const fullName = [user.givenName, user.familyName].filter(Boolean).join(' ') || user.name || "";
            setFormData(prev => ({
                ...prev,
                name: fullName,
                username: user.username || "",
                email: user.email || ""
            }))
        }
    }, [user])

    const [successMessage, setSuccessMessage] = useState("")
    const [errorMessage, setErrorMessage] = useState("")
    const [isLoading, setIsLoading] = useState(false)
    const [passwordData, setPasswordData] = useState({
        currentPassword: "",
        newPassword: "",
        confirmPassword: ""
    })

    const handleSave = async (e: React.FormEvent) => {
        e.preventDefault()
        setIsLoading(true)
        setSuccessMessage("")
        setErrorMessage("")

        try {
            
            const { name, email } = formData
            
            const nameParts = name.trim().split(' ')
            const lastName = nameParts.length > 1 ? nameParts.pop() || "" : ""
            const firstName = nameParts.join(' ')

            const profileResult = await updateProfile({
                name: firstName,
                surname: lastName,
                email
            })

            if (!profileResult.success) {
                setErrorMessage(profileResult.message || "Failed to update profile")
                setIsLoading(false)
                return
            }

            
            if (passwordData.newPassword) {
                if (passwordData.newPassword !== passwordData.confirmPassword) {
                    setErrorMessage("New passwords do not match")
                    setIsLoading(false)
                    return
                }

                if (!passwordData.currentPassword) {
                    setErrorMessage("Current password is required to set a new password")
                    setIsLoading(false)
                    return
                }

                const passwordResult = await changePassword({
                    currentPassword: passwordData.currentPassword,
                    newPassword: passwordData.newPassword
                })

                if (!passwordResult.success) {
                    setErrorMessage(passwordResult.message || "Failed to change password")
                    setIsLoading(false)
                    return
                }
            }

            setSuccessMessage("Profile updated successfully!")
            
            setPasswordData({ currentPassword: "", newPassword: "", confirmPassword: "" })

            
            if (refreshAuth) {
                await refreshAuth()
            }

        } catch (error) {
            setErrorMessage("An unexpected error occurred")
        } finally {
            setIsLoading(false)
        }
    }

    const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const file = e.target.files?.[0]
        if (file) {
            const reader = new FileReader()
            reader.onloadend = () => {
                setFormData({ ...formData, avatar: reader.result as string })
            }
            reader.readAsDataURL(file)
        }
    }

    return (
        <div className="container py-10 max-w-2xl">
            <h1 className="text-3xl font-bold tracking-tight mb-8">Edit Profile</h1>

            <form onSubmit={handleSave}>
                <Card>
                    <CardHeader>
                        <CardTitle>Profile Details</CardTitle>
                        <CardDescription>Update your personal information.</CardDescription>
                    </CardHeader>
                    <CardContent className="space-y-6">
                        {successMessage && (
                            <div className="flex items-center gap-2 p-4 text-sm text-green-600 bg-green-50 rounded-md">
                                <CheckCircle2 className="h-4 w-4" />
                                {successMessage}
                            </div>
                        )}

                        {errorMessage && (
                            <div className="flex items-center gap-2 p-4 text-sm text-red-600 bg-red-50 rounded-md">
                                <Trash className="h-4 w-4" />
                                {errorMessage}
                            </div>
                        )}

                        <div className="space-y-2">
                            <Label>Avatar</Label>
                            <div className="flex items-center gap-6">
                                <Avatar className="h-20 w-20">
                                    <AvatarImage src={formData.avatar} />
                                    <AvatarFallback>
                                        {formData.name
                                            ? formData.name.split(' ').map(n => n[0]).join('').toUpperCase().slice(0, 2)
                                            : 'U'}
                                    </AvatarFallback>
                                </Avatar>
                                <div className="space-y-2">
                                    <Input
                                        type="file"
                                        accept="image/*"
                                        onChange={handleFileChange}
                                        className="w-full max-w-xs"
                                        disabled={isLoading}
                                    />
                                    <p className="text-[0.8rem] text-muted-foreground">
                                        JPG, GIF or PNG. Max 1MB.
                                    </p>
                                </div>
                            </div>
                        </div>

                        <div className="space-y-2">
                            <Label htmlFor="username">Username</Label>
                            <Input
                                id="username"
                                value={formData.username}
                                disabled={true}
                                className="bg-muted cursor-not-allowed"
                            />
                            <p className="text-[0.8rem] text-muted-foreground">
                                Username cannot be changed.
                            </p>
                        </div>

                        <div className="space-y-2">
                            <Label htmlFor="name">Full Name</Label>
                            <Input
                                id="name"
                                value={formData.name}
                                onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                                disabled={isLoading}
                            />
                        </div>

                        <div className="space-y-2">
                            <Label htmlFor="email">Email</Label>
                            <Input
                                id="email"
                                type="email"
                                value={formData.email}
                                onChange={(e) => setFormData({ ...formData, email: e.target.value })}
                                disabled={isLoading}
                            />
                        </div>

                        <div className="pt-4 border-t">
                            <h3 className="text-lg font-medium mb-4">Change Password</h3>
                            <div className="space-y-4">
                                <div className="space-y-2">
                                    <Label htmlFor="currentPassword">Current Password</Label>
                                    <Input
                                        id="currentPassword"
                                        type="password"
                                        value={passwordData.currentPassword}
                                        onChange={(e) => setPasswordData({ ...passwordData, currentPassword: e.target.value })}
                                        disabled={isLoading}
                                    />
                                </div>
                                <div className="space-y-2">
                                    <Label htmlFor="newPassword">New Password</Label>
                                    <Input
                                        id="newPassword"
                                        type="password"
                                        value={passwordData.newPassword}
                                        onChange={(e) => setPasswordData({ ...passwordData, newPassword: e.target.value })}
                                        disabled={isLoading}
                                    />
                                </div>
                                <div className="space-y-2">
                                    <Label htmlFor="confirmPassword">Confirm New Password</Label>
                                    <Input
                                        id="confirmPassword"
                                        type="password"
                                        value={passwordData.confirmPassword}
                                        onChange={(e) => setPasswordData({ ...passwordData, confirmPassword: e.target.value })}
                                        disabled={isLoading}
                                    />
                                </div>
                            </div>
                        </div>
                    </CardContent>
                    <CardFooter className="flex justify-between">
                        <Button variant="ghost" asChild disabled={isLoading}>
                            <Link href="/profile">Cancel</Link>
                        </Button>
                        <Button type="submit" disabled={isLoading}>
                            {isLoading ? "Saving..." : "Save Changes"}
                        </Button>
                    </CardFooter>
                </Card>
            </form>
        </div>
    )
}
